using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;
public class PlantService : IPlantService
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILogger<PlantService> _logger;
    private readonly string _instanceName;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public PlantService(ILogger<PlantService> logger,
        IPlantRepository plantRepository,
        IOptions<InstanceOptions> instanceOptions,
        IConfiguration config,
        IMemoryCache cache)
    {
        _logger = logger;
        _plantRepository = plantRepository;
        _instanceName = instanceOptions.Value.InstanceName;
        _config = config;
        _cache = cache;
    }

    public List<string> GetPlantsHandledByInstance()
    {
        if (_cache.TryGetValue(_instanceName, out List<string> cachedPlants))
        {
            return cachedPlants;
        }

        var allPlants = _plantRepository.GetAllPlants();
        var plantsByInstances = GetPlantsByInstance();

        var plantsHandledByCurrentInstance = GetPlantsForInstance(plantsByInstances, _instanceName);
        if (plantsHandledByCurrentInstance.Any())
        {
            _logger.LogInformation($"[{_instanceName}] Plants configured for this instance: {string.Join(",", plantsHandledByCurrentInstance)}");

            var containsRemainingPlants = plantsHandledByCurrentInstance.Contains(PcsServiceBusInstanceConstants.RemainingPlants);
            if (containsRemainingPlants)
            {
                // We are also handling cases where RemainingPlants constant is used in combination with actual plants. E.g. PCS$TROLL_A, PCS$OSEBERG_C, REMAININGPLANTS. 
                var plantLeftovers = GetPlantLeftovers(plantsByInstances, allPlants, _instanceName);
                plantsHandledByCurrentInstance = plantsHandledByCurrentInstance.Union(plantLeftovers).ToList();
            }
            RemoveInvalidPlants(plantsHandledByCurrentInstance, allPlants, _instanceName);
        }

        if (!plantsHandledByCurrentInstance.Any())
        {
            var message = $"[{_instanceName}] No plants configured for this instance. Hence it will not start.";
            _logger.LogError(message);
            throw new Exception(message);
        }

        _cache.Set(_instanceName, plantsHandledByCurrentInstance, new MemoryCacheEntryOptions
        {
            // Have to restart instance to reload plants configuration.
            Priority = CacheItemPriority.NeverRemove
        });

        return plantsHandledByCurrentInstance;
    }


    private IEnumerable<string> GetPlantLeftovers(IEnumerable<PlantsByInstance> plantsByInstances, IEnumerable<string> allPlants, string instanceName)
    {
        var plantsForAllExceptInstance = GetPlantsForAllExceptInstance(plantsByInstances, instanceName);
        return GetPlantLeftovers(plantsForAllExceptInstance, allPlants);
    }

    private static IEnumerable<string> GetPlantsForAllExceptInstance(IEnumerable<PlantsByInstance> plantsByInstances, string instanceName) =>
        plantsByInstances
            .Where(x => x.InstanceName != instanceName)
            .SelectMany(x => x.Value.Split(',').Select(p => p.Trim()))
            .ToList();

    private static List<string> GetPlantsForInstance(IEnumerable<PlantsByInstance> plantsByInstances, string instanceName) =>
        plantsByInstances
            .Single(x => x.InstanceName == instanceName).Value.Split(',')
            .Select(p => p.Trim())
            .ToList();

    private void RemoveInvalidPlants(List<string> plantsHandledByCurrentInstance, IEnumerable<string> allPlants, string instanceName)
    {
        plantsHandledByCurrentInstance =
            plantsHandledByCurrentInstance.Except(PcsServiceBusInstanceConstants.AllPlantReplacementConstants).ToList();

        var plantsNotPresent = plantsHandledByCurrentInstance.Except(PcsServiceBusInstanceConstants.AllPlantConstants).Except(allPlants).ToList();
        if (plantsNotPresent.Any())
        {
            _logger.LogWarning(
                $"[{instanceName}] The following plant(s) is/ are not valid: {string.Join(",", plantsNotPresent)}. Removing them/ it from plants being processed.");
            plantsHandledByCurrentInstance = plantsHandledByCurrentInstance.Except(plantsNotPresent).ToList();
        }
        else
        {
            _logger.LogInformation($"[{instanceName}] Plants validated: {string.Join(",", plantsHandledByCurrentInstance)}");
        }
    }

    private static IEnumerable<string> GetPlantLeftovers(IEnumerable<string> handledPlants, IEnumerable<string> allNonVoidedPlants) =>
        allNonVoidedPlants.Except(handledPlants);

    public virtual List<PlantsByInstance> GetPlantsByInstance()
    {
        return _config.GetRequiredSection("PlantsByInstance").Get<List<PlantsByInstance>>();
    }
}
