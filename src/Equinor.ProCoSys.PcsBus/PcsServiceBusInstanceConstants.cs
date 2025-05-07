using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus;

public class PcsServiceBusInstanceConstants
{
    public const string Plant = "PLANT"; // Placeholder for all plants.
    public const string NoPlant = "NOPLANT"; // Placeholder for no plant.
    public const string RemainingPlants = "REMAININGPLANTS"; // Placeholder for all plants except the ones already resolved.
    public const string DefaultInstanceName = "UNIQUE"; // To indicate that this is the one and only instance in use.

    /*
     * All constants that can be used as placeholders.
     * Both the one that results in a replacement (RemainingPlants) and the ones that is resolved runtime (Plant, NoPlant).
     */
    public static readonly List<string> AllPlantConstants = new()
    {
        Plant,
        NoPlant,
        RemainingPlants
    };

    /*
     * Constants resulting in a replacement of the placeholder.
     */
    public static readonly List<string> AllPlantReplacementConstants = new()
    {
        RemainingPlants
    };

}
