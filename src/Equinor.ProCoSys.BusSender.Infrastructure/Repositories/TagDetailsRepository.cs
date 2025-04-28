﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

    public async Task<Dictionary<long, string>> GetDetailsByTagId(IEnumerable<long> tagIds)
    {
        var dbConnection = _context.Database.GetDbConnection();
        var connectionWasClosed = dbConnection.State != ConnectionState.Open;
        if (connectionWasClosed)
        {
            await _context.Database.OpenConnectionAsync();
        }

        try
        {
            // Initialize the dictionary with all tagIds and default value "{}"
            // to ensure that all tagIds are present with a default of {} when no details are found.
            var tagDetailsDictionary = tagIds.ToDictionary(id => id, _ => "{}");

            await using var command = _context.Database.GetDbConnection().CreateCommand();
            if (tagIds.Count()>1) {
                command.CommandText = GetTagDetailsQuery(tagIds);
            }
            else
            {
                command.CommandText = GetTagDetailsQuerySingle(tagIds.First());
            }

            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                _logger.LogDebug("No tag details found for any of the provided tag IDs. Returning empty set for all tag ids.");
                return tagDetailsDictionary;
            }

            while (await result.ReadAsync())
            {
                if (result[0] is DBNull)
                {
                    throw new InvalidOperationException("Tag id is null in query result. This should never happen.");
                }

                var tagId = result.GetInt64(0);
                var tagDetails = result.IsDBNull(1) ? "{}" : result.GetString(1);
                tagDetailsDictionary[tagId] = "{"+tagDetails+"}";
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

    private static string GetTagDetailsQuerySingle(long tagId) =>
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
                    join elementfield val on (val.field_id = def.field_id and val.element_id = {tagId})
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
