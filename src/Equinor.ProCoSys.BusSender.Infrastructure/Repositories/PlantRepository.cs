using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<string>> GetAllPlantsAsync()
    {
        var dbConnection = _context.Database.GetDbConnection();
        var connectionWasClosed = dbConnection.State != ConnectionState.Open;
        if (connectionWasClosed)
        {
            await _context.Database.OpenConnectionAsync();
        }

        try
        {
            await using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = GetAllPlantsQuery();

            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                _logger.LogInformation("No plants found in database.");
                return new List<string>();
            }

            var plants = new List<string>();

            while (await result.ReadAsync())
            {
                if (result[0] is not DBNull)
                {
                    plants.Add((string)result[0]);
                }
            }

            if (!plants.Any())
            {
                _logger.LogInformation("No plants found in database.");
            }

            return plants;
        }
        finally
        {
            //If we open it, we have to close it.
            if (connectionWasClosed)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }

    private static string GetAllPlantsQuery() =>
        @$"
            select projectschema
            from projectschema
            where isvoided = 'N'
        ";
}
