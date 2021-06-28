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
    }
}
