using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IPlantRepository
{
    Task<List<string>> GetAllPlantsAsync();
}
