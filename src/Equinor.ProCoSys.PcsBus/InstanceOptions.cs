using System.ComponentModel.DataAnnotations;

namespace Equinor.ProCoSys.PcsServiceBus;
public class InstanceOptions
{
    [Required]
    public string InstanceName { get; set; } = PcsServiceBusInstanceConstants.DefaultInstanceName;

    [Required]
    public int MessageChunkSize { get; set; } = 200;
}
