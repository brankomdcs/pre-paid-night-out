using System.Collections.ObjectModel;
using System.Fabric.Description;

namespace Orchestrator
{
    public class OrchestratorMetrics : KeyedCollection<string, ServiceLoadMetricDescription>
    {
        protected override string GetKeyForItem(ServiceLoadMetricDescription item)
        {
            return item.Name;
        }
    }
}
