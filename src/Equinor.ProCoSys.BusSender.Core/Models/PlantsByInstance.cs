using System.ComponentModel.DataAnnotations;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
public class PlantsByInstance
{
    [Required]
    public string InstanceName { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;
}
