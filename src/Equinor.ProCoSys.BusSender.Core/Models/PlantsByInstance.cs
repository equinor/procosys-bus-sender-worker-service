using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
public class PlantsByInstance
{
    public string InstanceName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
