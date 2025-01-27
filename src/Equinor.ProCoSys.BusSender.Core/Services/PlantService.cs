using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Logging;


namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;
public class PlantService : IPlantService
{
    private readonly ILogger<PlantService> _logger;
    private List<string> _plantsHandledByCurrentInstance = new();

    public PlantService(ILogger<PlantService> logger) => _logger = logger;

    public List<string> GetPlantsHandledByInstance(List<PlantsByInstance>? plantsByInstances, List<string> allPlants, string instanceName)
    {
        _plantsHandledByCurrentInstance = GetPlantsForInstance(plantsByInstances, instanceName);
        if (_plantsHandledByCurrentInstance.Any())
        {
            _logger.LogInformation($"[{instanceName}] Plants configured for this instance: {string.Join(",", _plantsHandledByCurrentInstance)}");

            var containsRemainingPlants = _plantsHandledByCurrentInstance.Contains(PcsServiceBusInstanceConstants.RemainingPlants);
            if (containsRemainingPlants)
            {
                // We are also handling cases where RemainingPlants constant is used in combination with actual plants. E.g. PCS$TROLL_A, PCS$OSEBERG_C, REMAININGPLANTS. 
                AddPlantLeftovers(plantsByInstances, allPlants, instanceName);
            }
            RemoveInvalidPlants(allPlants, instanceName);
        }

        if (!_plantsHandledByCurrentInstance.Any())
        {
            var message = $"[{instanceName}] No plants configured for this instance. Hence it will not start.";
            _logger.LogError(message);
            throw new Exception(message);
        }

        return _plantsHandledByCurrentInstance;
    }


    private void AddPlantLeftovers(IEnumerable<PlantsByInstance> plantsByInstances, IEnumerable<string> allPlants, string instanceName)
    {
        var plantsForAllExceptInstance = GetPlantsForAllExceptInstance(plantsByInstances, instanceName);
        var plantLeftovers = GetPlantLeftovers(plantsForAllExceptInstance, allPlants);
        _plantsHandledByCurrentInstance = _plantsHandledByCurrentInstance.Union(plantLeftovers).ToList();
    }

    private static IEnumerable<string> GetPlantsForAllExceptInstance(IEnumerable<PlantsByInstance> plantsByInstances, string instanceName) =>
        plantsByInstances
            .Where(x => x.InstanceName != instanceName)
            .SelectMany(x => x.Value.Split(','))
            .ToList();

    private static List<string> GetPlantsForInstance(IEnumerable<PlantsByInstance> plantsByInstances, string instanceName) =>
        plantsByInstances
            .First(x => x.InstanceName == instanceName).Value.Split(',')
            .ToList();

    private void RemoveInvalidPlants(IEnumerable<string> allPlants, string instanceName)
    {
        _plantsHandledByCurrentInstance =
            _plantsHandledByCurrentInstance.Except(PcsServiceBusInstanceConstants.AllPlantReplacementConstants).ToList();

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

    private static IEnumerable<string> GetPlantLeftovers(IEnumerable<string> handledPlants, IEnumerable<string> allNonVoidedPlants) =>
        allNonVoidedPlants.Except(handledPlants);
}
