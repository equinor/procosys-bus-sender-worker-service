using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus;
public class InstanceConfig
{
    public List<string> PlantsHandledByCurrentInstance { get; set; } = new();
}
