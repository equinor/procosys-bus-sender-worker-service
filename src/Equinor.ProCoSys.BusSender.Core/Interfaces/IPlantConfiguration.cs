using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure;
public interface IPlantConfiguration
{
    void RegisterConfiguredPlantsAsync(IHost host);
    List<ConfigurationSetting> GetPlants();
}
