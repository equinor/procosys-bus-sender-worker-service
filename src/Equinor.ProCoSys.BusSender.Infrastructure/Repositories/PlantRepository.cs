﻿using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class PlantRepository : IPlantRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<PlantRepository> _logger;

    public PlantRepository(BusSenderServiceContext context, ILogger<PlantRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<string> GetAllPlants() =>  
        _context.Plants
            .Where(p => p.IsVoided == "N")
            .Select(p => p.ProjectSchema)
            .ToList();


}
