using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
public class PlantsByInstance
{
    [Required]
    public string InstanceName { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;
}
