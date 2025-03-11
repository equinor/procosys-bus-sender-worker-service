using System;
using System.Text.Json.Serialization;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
public class PlantLease
{
    public DateTime? LeaseExpiry { get; set; }
    public required string Plant { get; set; }
    public DateTime? LastProcessed { get; set; }

    [JsonIgnore]
    // Flag to tell if this plant is the one being handled by the current instance.
    // This is transient and not stored in the blob.
    public bool IsCurrent { get; set; } = false;
}
