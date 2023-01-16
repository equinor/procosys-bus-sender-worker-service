using System;
using System.Data;

using Dapper;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{

    public static readonly GuidTypeHandler Default = new();

    public override void SetValue(IDbDataParameter parameter, Guid value) => throw new NotImplementedException();

    public override Guid Parse(object value)
    {
        var inVal = (byte[])value; return new Guid(inVal);
    }
}
