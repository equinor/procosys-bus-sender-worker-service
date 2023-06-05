using System;
using System.Data;
using System.Text;
using Dapper;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public static readonly GuidTypeHandler Default = new();

    public override Guid Parse(object value)
    {
        var result = Guid.TryParse(value.ToString(), out var guid)
            ? guid
            : Guid.TryParse(ByteArrayToHexString((byte[])value), out var guid2)
                ? guid2
                : new Guid((byte[])value);
        return result;
    }

    public override void SetValue(IDbDataParameter parameter, Guid value)
        => throw new NotImplementedException();
    
    private static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
        {
            result.Append(b.ToString("x2"));
        }

        return result.ToString();
    }
}
