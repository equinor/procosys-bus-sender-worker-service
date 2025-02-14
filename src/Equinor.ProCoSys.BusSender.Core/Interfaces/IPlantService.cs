using System.Collections.Generic;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
public interface IPlantService
{
    List<string> GetPlantsHandledByInstance(List<PlantLease> plantLeases);
    List<string>? GetAllPlants();
}
