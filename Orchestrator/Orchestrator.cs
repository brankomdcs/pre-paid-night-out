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
using System.Net.Http;
using System.Fabric.Description;

namespace Orchestrator
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Orchestrator : StatelessService
    {
        private static int numberOfRequestsWithinMinute = 0;
        private const string requestsPerMinuteMetricName = "OrchestratorRequestsPerMin";
        public Orchestrator(StatelessServiceContext context)
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
                new ServiceInstanceListener(
                    serviceContext =>
                        new KestrelCommunicationListener(
                            serviceContext,
                            "ServiceEndpoint",
                            (url, listener) =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                                return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<HttpClient>(new HttpClient())
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
            DefineOrchestratorMetrics();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Partition.ReportLoad(new List<LoadMetric> { new LoadMetric(requestsPerMinuteMetricName, numberOfRequestsWithinMinute) });
                numberOfRequestsWithinMinute = 0;
                
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
        }

        private void DefineOrchestratorMetrics()
        {
            StatelessServiceUpdateDescription updateServiceDescription = new StatelessServiceUpdateDescription();
            StatelessServiceLoadMetricDescription requestsPerSecondMetric = new StatelessServiceLoadMetricDescription() { Name = requestsPerMinuteMetricName, Weight = ServiceLoadMetricWeight.High };

            if (updateServiceDescription.Metrics == null)
                updateServiceDescription.Metrics = new OrchestratorMetrics();

            updateServiceDescription.Metrics.Add(requestsPerSecondMetric);

            FabricClient fabricClient = new FabricClient();
            fabricClient.ServiceManager.UpdateServiceAsync(GetOrchestratorServiceNameFrom(Context), updateServiceDescription);
        }

        public static void RegisterRequestForMetrics() { numberOfRequestsWithinMinute++; }

        private const string reverseProxyAddress = "http://localhost:19081";
        private static string GetApplicationBaseUriFrom(ServiceContext context) => context.CodePackageActivationContext.ApplicationName;

        internal static Uri GetOrchestratorServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Orchestrator");

        internal static Uri GetAccountServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Account");
        internal static Uri GetAccountServiceAddressFrom(Uri serviceName) => new Uri($"{reverseProxyAddress}{serviceName.AbsolutePath}");

        internal static Uri GetTransactionServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Transaction");
        internal static Uri GetTransactionServiceAddressFrom(Uri serviceName) => new Uri($"{reverseProxyAddress}{serviceName.AbsolutePath}");

        internal static Uri GetPaymentServiceNameFrom(ServiceContext context) => new Uri($"{GetApplicationBaseUriFrom(context)}/Payment");
        internal static Uri GetPaymentServiceAddressFrom(Uri serviceName) => new Uri($"{reverseProxyAddress}{serviceName.AbsolutePath}");
    }
}
