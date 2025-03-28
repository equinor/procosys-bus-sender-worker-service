using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;
public class PlantService : IPlantService
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILogger<PlantService> _logger;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public PlantService(ILogger<PlantService> logger,
        IPlantRepository plantRepository,
        IConfiguration config,
        IMemoryCache cache)
    {
        _logger = logger;
        _plantRepository = plantRepository;
        _config = config;
        _cache = cache;
    }

    public virtual List<string>? GetAllPlants()
    {
        if (!_cache.TryGetValue("AllPlants", out List<string>? allPlants))
        {
            _logger.LogDebug("Retrieving plants from memory cache.");
            allPlants = _plantRepository.GetAllPlants();
            if (allPlants == null || !allPlants.Any())
            {
                var message = "No plants found in database.";
                _logger.LogError(message);
                throw new Exception(message);
            }
            _cache.Set("AllPlants", allPlants, new MemoryCacheEntryOptions
            {
                // Have to restart instance to reload plants configuration.
                Priority = CacheItemPriority.NeverRemove
            });
            _logger.LogDebug("Plants read from database and added to memory cache.");
        }
        else
        {
            _logger.LogDebug("Plants retrieved from memory cache.");
        }

        return allPlants;
    }

    public List<string> GetPlantsForCurrent(List<PlantLease> plantLeases)
    {
        var plantsHandledByCurrentInstance = new List<string>();
        var allPlants = GetAllPlants();

        if (allPlants == null || !allPlants.Any())
        {
            throw new Exception("No plants found in database.");
        }

        var plant = plantLeases.First(x => x.IsCurrent).Plant;
        var plants = plantLeases.Where(x => x.IsCurrent)
                                .SelectMany(x => x.Plant.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                .ToList();
        if (!string.IsNullOrEmpty(plant))
        {
            _logger.LogInformation($"Handling messages for plant: {plant}");

            var otherDefinedPlants = plantLeases.Where(x => !x.IsCurrent)
                                    .SelectMany(x => x.Plant.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                    .ToList();
            if (plants.Contains(PcsServiceBusInstanceConstants.RemainingPlants))
            {
                // We are also handling cases where RemainingPlants constant is used in combination with actual plants. E.g. PCS$TROLL_A, PCS$OSEBERG_C, REMAININGPLANTS. 
                var plantLeftovers = GetPlantLeftovers(otherDefinedPlants, allPlants);
                plantsHandledByCurrentInstance = plants.Union(plantLeftovers).ToList();
                RemovePlantReplacement(plantsHandledByCurrentInstance);
            }
            else
            {
                plantsHandledByCurrentInstance.AddRange(plants);
            }

            if (otherDefinedPlants.Intersect(plants).Any())
            {
                var message = "One or more plants are defined for multiple items. Check plantslease blob.";
                _logger.LogError(message);
                throw new Exception(message);
            }
            RemoveInvalidPlants(plantsHandledByCurrentInstance, allPlants);
        }

        if (!plantsHandledByCurrentInstance.Any())
        {
            var message = "No valid plants to handle for this configuration item. Check plantslease blob. E.g. has non valid plants been included?";
            _logger.LogError(message);
            throw new Exception(message);
        }

        return plantsHandledByCurrentInstance;
    }

    private void RemoveInvalidPlants(List<string> plantsHandledByCurrentInstance, IEnumerable<string> allPlants)
    {
        var invalidPlants = plantsHandledByCurrentInstance
            .Except(PcsServiceBusInstanceConstants.AllPlantReplacementConstants)
            .Except(PcsServiceBusInstanceConstants.AllPlantConstants)
            .Except(allPlants)
            .ToList();

        foreach (var plant in invalidPlants)
        {
            plantsHandledByCurrentInstance.Remove(plant);
        }

        if (invalidPlants.Any())
        {
            _logger.LogWarning($"These plants are not valid: {string.Join(", ", invalidPlants)}");
        }
    }

    private void RemovePlantReplacement(List<string> plants) => plants.RemoveAll(x => PcsServiceBusInstanceConstants.AllPlantReplacementConstants.Contains(x));

    private static IEnumerable<string> GetPlantLeftovers(IEnumerable<string> handledPlants, IEnumerable<string> allNonVoidedPlants) =>
        allNonVoidedPlants.Except(handledPlants);

}
