using System;
using System.Data;
using Dapper;
namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;


internal class DateTimeUtcHandler : SqlMapper.TypeHandler<DateTime>
{
    public static readonly DateTimeUtcHandler Default = new();
    public override void SetValue(IDbDataParameter parameter, DateTime value) 
        => parameter.Value = value;

    public override DateTime Parse(object value) 
        => DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
}
