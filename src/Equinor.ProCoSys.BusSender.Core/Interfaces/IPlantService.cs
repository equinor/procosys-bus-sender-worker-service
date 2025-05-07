using System.Collections.Generic;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
public interface IPlantService
{
    List<string> GetPlantsForCurrent(List<PlantLease> plantLeases);
    List<string>? GetAllPlants();
}
