using System;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using Dapper;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;

public class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    public static readonly DateOnlyHandler Default = new();

    public override void SetValue(IDbDataParameter parameter, DateOnly value) 
        => parameter.Value = value.ToString("yyyy-MM-dd");

    public override DateOnly Parse(object value)
    {
        var dateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        //var localTime = dateTime.ToUniversalTime();
        return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}
