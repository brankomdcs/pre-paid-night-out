using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;

namespace Common
{
    public class ResourceConfigurationManager
    {
        private readonly FabricClient fabricClient;
        private readonly Uri applicationName;

        public ResourceConfigurationManager(FabricClient fabricClient, Uri applicationName)
        {
            this.fabricClient = fabricClient;
            this.applicationName = applicationName;
        }

        public bool IsSetToUseConfigurationFromCode(StatelessServiceContext context)
        {
            try
            {
                var settings = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
                return bool.Parse(settings.Settings.Sections["ResourceConfigurationSettings"].Parameters["UseConfigurationFromCode"].Value);
            }
            catch (KeyNotFoundException)
            {
                //In case there is no settings defined in Settings.xml to disable configuration from code, return true:
                return true;
            }
        }

        public void AddMetric(ServiceLoadMetricDescription serviceLoadMetricDescription) 
        {
            StatelessServiceUpdateDescription updateServiceDescription = new StatelessServiceUpdateDescription();

            if (updateServiceDescription.Metrics == null)
            {
                updateServiceDescription.Metrics = new MetricsCollection();
            }
            updateServiceDescription.Metrics.Add(serviceLoadMetricDescription);

            fabricClient.ServiceManager.UpdateServiceAsync(applicationName, updateServiceDescription);
        }

        public void AddScalingPolicy(PartitionInstanceCountScaleMechanism partitionInstanceCountScaleMechanism, AveragePartitionLoadScalingTrigger averagePartitionLoadScalingTrigger)
        {
            ScalingPolicyDescription policy = new ScalingPolicyDescription(partitionInstanceCountScaleMechanism, averagePartitionLoadScalingTrigger);
            StatelessServiceUpdateDescription updateServiceDescription = new StatelessServiceUpdateDescription();

            if (updateServiceDescription.ScalingPolicies == null)
            {
                updateServiceDescription.ScalingPolicies = new List<ScalingPolicyDescription>();
            }
            updateServiceDescription.ScalingPolicies.Add(policy);

            fabricClient.ServiceManager.UpdateServiceAsync(applicationName, updateServiceDescription);
        }

        public void AddAffinity(ServiceCorrelationDescription serviceCorrelationDescription) 
        {
            StatelessServiceUpdateDescription updateServiceDescription = new StatelessServiceUpdateDescription();

            if (updateServiceDescription.Correlations == null) 
            {
                updateServiceDescription.Correlations = new List<ServiceCorrelationDescription>();
            }
            updateServiceDescription.Correlations.Add(serviceCorrelationDescription);

            fabricClient.ServiceManager.UpdateServiceAsync(applicationName, updateServiceDescription);
        }
    }
}
