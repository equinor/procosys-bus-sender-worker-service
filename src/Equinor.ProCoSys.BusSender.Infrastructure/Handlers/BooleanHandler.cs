using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;

internal class BooleanHandler : SqlMapper.TypeHandler<bool>
{
    public static readonly BooleanHandler Default = new();

    public override bool Parse(object value) => value.ToString() == "Y";

    public override void SetValue(IDbDataParameter parameter, bool value) => parameter.Value = value ? 'Y' : 'N';
}
