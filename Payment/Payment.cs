using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Common;
using System.Fabric.Description;

namespace Payment
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Payment : StatelessService
    {
        private static long numberOfRequestsWithinMinute = 0;
        private static long totalDurationOfRequestsWithinMinute = 0;
        private const string averageRequestTimeName = "PaymentAverageRequestTime";
        public Payment(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            DefineMetricsAndPolicies();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int averageDurationInMilliseconds = numberOfRequestsWithinMinute == 0 ? 0 : Convert.ToInt32(totalDurationOfRequestsWithinMinute / numberOfRequestsWithinMinute);
                Partition.ReportLoad(new List<LoadMetric> { new LoadMetric(averageRequestTimeName, averageDurationInMilliseconds) });
                numberOfRequestsWithinMinute = 0;
                totalDurationOfRequestsWithinMinute = 0;

                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
        }

        private void DefineMetricsAndPolicies()
        {
            ResourceConfigurationManager configurationManager = new ResourceConfigurationManager(new FabricClient(), GetPaymentServiceNameFrom(Context));

            StatelessServiceLoadMetricDescription requestsPerSecondMetric = new StatelessServiceLoadMetricDescription
            {
                Name = averageRequestTimeName,
                Weight = ServiceLoadMetricWeight.High
            };

            configurationManager.AddMetric(requestsPerSecondMetric);

            PartitionInstanceCountScaleMechanism mechanism = new PartitionInstanceCountScaleMechanism
            {
                MaxInstanceCount = 2,
                MinInstanceCount = 1,
                ScaleIncrement = 1
            };

            AveragePartitionLoadScalingTrigger trigger = new AveragePartitionLoadScalingTrigger
            {
                MetricName = averageRequestTimeName,
                ScaleInterval = TimeSpan.FromMinutes(1),
                LowerLoadThreshold = 500,
                UpperLoadThreshold = 1000
            };

            configurationManager.AddScalingPolicy(mechanism, trigger);

            ServiceCorrelationDescription affinityDescription = new ServiceCorrelationDescription()
            {
                Scheme = ServiceCorrelationScheme.Affinity,
                ServiceName = GetOrchestratorServiceNameFrom(Context)
            };

            configurationManager.AddAffinity(affinityDescription);
        }

        public static void RegisterRequestForMetrics(long elapsedMiliseconds) {
            numberOfRequestsWithinMinute++;
            totalDurationOfRequestsWithinMinute += elapsedMiliseconds;
        }

        private static string GetApplicationBaseUriFrom(ServiceContext context) => context.CodePackageActivationContext.ApplicationName;
        internal static Uri GetPaymentServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Payment");
        internal static Uri GetOrchestratorServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Orchestrator");
    }
}
