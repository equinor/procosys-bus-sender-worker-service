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
            _logger.LogInformation("Retrieving plants from memory cache.");
            allPlants = _plantRepository.GetAllPlants();
            _cache.Set("AllPlants", allPlants, new MemoryCacheEntryOptions
            {
                // Have to restart instance to reload plants configuration.
                Priority = CacheItemPriority.NeverRemove
            });
            _logger.LogInformation("Plants read from database and added to memory cache.");
        }
        else
        {
            _logger.LogInformation("Plants retrieved from memory cache.");
        }

        return allPlants;
    }

    public List<string> GetPlantsHandledByInstance(List<PlantLease> plantLeases)
    {
        var plantsHandledByCurrentInstance = new List<string>();
        var allPlants = GetAllPlants();

        if (allPlants != null && allPlants.Any())
        {
            var plant = plantLeases.First(x => x.IsCurrent).Plant;
            if (!string.IsNullOrEmpty(plant))
            {
                _logger.LogInformation($"Handling messages for plant: {plant}");

                if (plant.Equals(PcsServiceBusInstanceConstants.RemainingPlants))
                {
                    var definedPlants = plantLeases.Where(x => !x.IsCurrent).Select(x => x.Plant).ToList();
                    // We are also handling cases where RemainingPlants constant is used in combination with actual plants. E.g. PCS$TROLL_A, PCS$OSEBERG_C, REMAININGPLANTS. 
                    var plantLeftovers = GetPlantLeftovers(definedPlants, allPlants);
                    plantsHandledByCurrentInstance = plantsHandledByCurrentInstance.Union(plantLeftovers).ToList();
                }
                else
                {
                    plantsHandledByCurrentInstance.Add(plant);
                }

                RemoveInvalidPlants(plantsHandledByCurrentInstance, allPlants);
            }

            if (!plantsHandledByCurrentInstance.Any())
            {
                var message = "No plants to handle.";
                _logger.LogError(message);
                throw new Exception(message);
            }
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

    private static IEnumerable<string> GetPlantLeftovers(IEnumerable<string> handledPlants, IEnumerable<string> allNonVoidedPlants) =>
        allNonVoidedPlants.Except(handledPlants);

}
