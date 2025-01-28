using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Validation
{
    public static class ConfigurationValidator
    {
        public static void ValidatePlantsByInstance(List<PlantsByInstance>? plantsByInstances)
        {
            if (plantsByInstances == null || !plantsByInstances.Any())
            {
                throw new InvalidOperationException("PlantsByInstances is not defined in configuration. Exiting.");
            }

            foreach (var instance in plantsByInstances)
            {
                if (string.IsNullOrEmpty(instance.InstanceName) || string.IsNullOrEmpty(instance.Value))
                {
                    throw new InvalidOperationException("PlantsByInstances is not defined correctly in configuration. Exiting.");
                }
            }
        }

        public static void ValidateInstanceOptions(InstanceOptions options)
        {
            if (string.IsNullOrEmpty(options.InstanceName))
            {
                throw new InvalidOperationException("InstanceName is required in configuration.");
            }

            if (options.MessageChunkSize <= 0)
            {
                throw new InvalidOperationException("MessageChunkSize must be greater than 0.");
            }
        }
    }
}
