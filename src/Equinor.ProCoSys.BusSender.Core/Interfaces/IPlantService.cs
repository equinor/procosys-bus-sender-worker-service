using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
public interface IPlantService
{
    void RegisterPlantsHandledByCurrentInstance(IHost host, List<string> allPlants);
    List<string> GetPlantsHandledByCurrentInstance();
    IConfiguration GetConfiguration();
}
