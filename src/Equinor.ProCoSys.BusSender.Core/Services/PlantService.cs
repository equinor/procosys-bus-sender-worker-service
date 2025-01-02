using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;
public class PlantService : IPlantService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PlantService> _logger;
    private List<string> _plantsHandledByCurrentInstance = new();

    public List<string> GetPlantsHandledByCurrentInstance() => _plantsHandledByCurrentInstance;
    public IConfiguration GetConfiguration() => _configuration;

    public PlantService(IConfiguration configuration, ILogger<PlantService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected virtual async Task<List<string>> RegisterHandledPlantsAsync(IHost host)
    {
        var appConfigurationName = _configuration["Azure:AppConfig"];
        var endpoint = $"https://{appConfigurationName}.azconfig.io";

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            throw new UriFormatException("The URI scheme is not valid.");
        }

        var client = new ConfigurationClient(new Uri(endpoint), new DefaultAzureCredential());
        var settingKey = "MessageSites";
        var selector = new SettingSelector { KeyFilter = settingKey, LabelFilter = "*" };
        var allHandledPlants = new List<string>();

        await foreach (var setting in client.GetConfigurationSettingsAsync(selector))
        {
            var plants = setting.Value.Split(',').ToList();
            allHandledPlants.AddRange(plants);
        }
        return allHandledPlants;
    }

    public void RegisterPlantsHandledByCurrentInstance(IHost host, List<string> allPlants)
    {
        var plantsString = _configuration["MessageSites"];
        var instanceName = _configuration["InstanceName"];
        if (!string.IsNullOrWhiteSpace(plantsString))
        {
            _logger.LogInformation($"[{instanceName}] Plants configured for this instance: {plantsString}");
            _plantsHandledByCurrentInstance = plantsString.Split(',').ToList();
            if (_plantsHandledByCurrentInstance.Contains(PcsServiceBusInstanceConstants.RemainingPlants))
            {
                var allHandedPlants = RegisterHandledPlantsAsync(host).Result;

                // We are also handling cases where RemainingPlants constant is used in combination with actual plants. E.g. PCS$TROLL_A, PCS$OSEBERG_C, REMAININGPLANTS. 
                _plantsHandledByCurrentInstance = _plantsHandledByCurrentInstance.Union(CalcPlantLeftovers(allHandedPlants, allPlants)).ToList();
            }
            WashPlants(allPlants, instanceName);
        }
        else
        {
            _plantsHandledByCurrentInstance = new List<string>();
            _logger.LogWarning($"[{instanceName}] No plants configured for this instance. Hence it will be idle.");
        }
    }

    private void WashPlants(List<string> allPlants, string instanceName)
    {
        _plantsHandledByCurrentInstance =
            _plantsHandledByCurrentInstance.Except(PcsServiceBusInstanceConstants.AllPlantResolventConstants).ToList();

        var plantsNotPresent = _plantsHandledByCurrentInstance.Except(PcsServiceBusInstanceConstants.AllPlantConstants).Except(allPlants).ToList();
        if (plantsNotPresent.Any())
        {
            _logger.LogWarning(
                $"[{instanceName}] The following plant(s) is/ are not valid: {string.Join(",", plantsNotPresent)}. Removing them/ it from plants being processed.");
            _plantsHandledByCurrentInstance = _plantsHandledByCurrentInstance.Except(plantsNotPresent).ToList();
        }
        else
        {
            _logger.LogInformation($"[{instanceName}] Plants validated: {string.Join(",", _plantsHandledByCurrentInstance)}");
        }
    }

    private static IEnumerable<string> CalcPlantLeftovers(IEnumerable<string> handledPlants, IEnumerable<string> allNonVoidedPlants) => allNonVoidedPlants.Except(handledPlants);
}
