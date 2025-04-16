using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class TagDetailsRepository : ITagDetailsRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<TagDetailsRepository> _logger;

    public TagDetailsRepository(BusSenderServiceContext context, ILogger<TagDetailsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetDetailsStringByTagId(long tagId)
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
            command.CommandText = GetTagDetailsQuery([tagId]);

            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                _logger.LogInformation("Tag with id {TagId} did not have any tagDetails", tagId);
                return "{}";
            }

            if (!await result.ReadAsync() || result[0] is DBNull)
            {
                return "{}";
            }

            var tagDetails = (string)result[0];

            if (await result.ReadAsync())
            {
                _logger.LogError("TagDetails returned more than 1 row, this should not happen");
            }

            return "{" + tagDetails + "}";
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


    public async Task<Dictionary<long, string>> GetDetailsListByTagId(IEnumerable<long> tagIds)
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
            command.CommandText = GetTagDetailsQuery(tagIds);

            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                _logger.LogInformation("No tag details found for the provided tag IDs.");
                return new Dictionary<long, string>();
            }

            var tagDetailsDictionary = new Dictionary<long, string>();

            while (await result.ReadAsync())
            {
                if (result[0] is DBNull)
                {
                    continue;
                }

                var tagId = result.GetInt64(0);
                var tagDetails = result.GetString(1);
                tagDetailsDictionary[tagId] = tagDetails;
            }

            return tagDetailsDictionary;
        }
        finally
        {
            // If we opened it, we have to close it.
            if (connectionWasClosed)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }

    private static string GetTagDetailsQuery(IEnumerable<long> tagIds) =>
        @$"
            select tagId, listagg('""'|| colName ||'"":""'|| regexp_replace(val, '([""\])', '\\\1') ||'""'
            || case when colname2 is not null then ',' || '""'|| colName2 ||'"":""'|| val2 ||'""' end
            || case when colname3 is not null then ',' || '""'|| colName3 ||'"":""'|| val3 ||'""' end,
            ',')
            within group (order by colName, colName2, colName3) as tagDetails
            from (
                select
                val.element_id tagId,
                decode(f.columnname, 'FROM_TAG_NUMBER', 'FromTagGuid', null) as colName2,
                decode(f.columnname, 'TO_TAG_NUMBER', 'ToTagGuid', null) as colName3,
                f.columnname as colName,
                coalesce(
                    val.valuestring,
                    to_char(val.valuedate, 'yyyy-mm-dd hh24:mi:ss'),
                    to_char(val.valuenumber),
                    tag.tagno,
                    libval.code
                ) as val,
                tag.procosys_guid as val2,
                tag.procosys_guid as val3
                from defineelementfield def
                    left join field f on def.field_id = f.field_id
                    left join library unit on unit.library_id = f.unit_id
                    join elementfield val on (val.field_id = def.field_id and val.element_id IN ({string.Join(",", tagIds)}))
                    join tag t on t.tag_id = val.element_id 
                    left join library libval on libval.library_id = val.library_id
                    left join library reg on reg.library_id = def.register_id
                    left join tag on tag.tag_id = val.tag_id
                where def.elementtype = 'TAG'
                and (def.register_id is null or def.register_id = t.register_id)
                and not def.isvoided = 'Y'
                and f.columntype in ('NUMBER', 'DATE', 'STRING', 'LIBRARY', 'TAG')
                and f.projectschema = t.projectschema
                order by def.sortkey nulls first
            )
            group by tagId
        ";
}
