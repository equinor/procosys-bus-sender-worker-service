using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Infrastructure.Data;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories
{
    public class TagDetailsRepository : ITagDetailsRepository
    {
        private readonly BusSenderServiceContext _context;

        public TagDetailsRepository(BusSenderServiceContext context)
        {
            _context = context;
        }


        public async Task<string> GetByTagId(long tagId)
        {

            await using var context = _context;
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = GetTagDetailsQuery(tagId);
            await context.Database.OpenConnectionAsync();
            await using var result = await command.ExecuteReaderAsync();

            if (!result.HasRows)
            {
                return null;
            }

            var tagDetails =   (string)result[0];
             
            return tagDetails;

        }

        private static string GetTagDetailsQuery(long tagId) =>
        @$"
        select listagg('{{""columnname"":""'|| colname ||'"",""value"":""'|| val ||'""}}', ',')
            within group (order by colname) as tagdetails 
            from (
                select
                    f.columnname as colname,
                    coalesce(
                    regexp_replace(val.valuestring, '([""\])', '\\\1'),
                    to_char(val.valuedate, 'yyyy-mm-dd hh:mm:ss'),
                    to_char(val.valuenumber),
                    tag.tagno,
                    regexp_replace(libval.code, '([""\])', '\\\1')
                ) as val
                from defineelementfield def
                    left join field f on def.field_id = f.field_id
                    left join library unit on unit.library_id = f.unit_id
                    join elementfield val on (val.field_id = def.field_id and val.element_id = {tagId})
                    join tag t on t.tag_id = val.element_id  
                    left join library libval on libval.library_id = val.library_id
                    left join library reg on reg.library_id = def.register_id
                    left join tag on tag.tag_id = val.tag_id
                where def.elementtype = 'tag'
                and (def.register_id is null or def.register_id = t.register_id)
                and not def.isvoided = 'y'
                and f.columntype in ('number', 'date', 'string', 'library', 'tag')
                and f.projectschema = t.projectschema
            order by def.sortkey nulls first
        )";
    }
}
