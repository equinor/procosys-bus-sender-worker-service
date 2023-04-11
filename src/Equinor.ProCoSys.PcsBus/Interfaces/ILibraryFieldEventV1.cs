using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ILibraryFieldEventV1
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid LibraryGuid { get; set; }
    string? LibraryType { get; set; }
    string Code { get; set; }
    string ColumnName { get; set; }
    string? ColumnType { get; set; }
    string? StringValue { get; set; }
    DateOnly? DateValue { get; set; }
    double? NumberValue { get; set; }
    Guid? LibraryValueGuid { get; set; }
    DateTime LastUpdated { get; set; }
}
