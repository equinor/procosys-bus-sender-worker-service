using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus;

public class PcsServiceBusInstanceConstants
{
    public const string Plant = "PLANT";
    public const string NoPlant = "NOPLANT";
    public const string RemainingPlants = "REMAININGPLANTS";
    public const string DefaultInstanceName = "UNIQUE";

    public static readonly List<string> AllPlantConstants = new()
    {
        Plant,
        NoPlant,
        RemainingPlants
    };

    public static readonly List<string> AllPlantResolventConstants = new()
    {
        RemainingPlants
    };

}
