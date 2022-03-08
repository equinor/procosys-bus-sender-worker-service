using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories
{
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
            await using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = GetTagDetailsQuery(tagId);
            await _context.Database.OpenConnectionAsync();
            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                _logger.LogInformation($"Tag with id {tagId} did not have any tagDetails");
                return null;
            }

            if (!await result.ReadAsync() || result[0] is DBNull)
            {
                return "";
            }

            var tagDetails = (string)result[0];

                
            if (await result.ReadAsync())
            {
                _logger.LogError("TagDetails returned more than 1 row, this should not happen.");
            }

            return "{"+ tagDetails +"}";
        }

        private static string GetTagDetailsQuery(long tagId) =>
        @$"
        select listagg('""'|| colName ||'"":""'|| regexp_replace(val, '([""\])', '\\\1') ||'""', ',')
            within group (order by colName) as tagDetails 
            from (
                select
                    f.columnname as colName,
                    coalesce(
                    val.valuestring,
                    to_char(val.valuedate, 'YYYY-MM-DD hh:mm:ss'),
                    to_char(val.valuenumber),
                    tag.tagno,
                    libval.code
                ) as val
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
        )";
    }
}
