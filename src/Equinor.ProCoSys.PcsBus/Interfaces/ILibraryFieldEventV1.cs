using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ILibraryFieldEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    Guid LibraryGuid { get; init; }
    string? LibraryType { get; init; }
    string Code { get; init; }
    string ColumnName { get; init; }
    string? ColumnType { get; init; }
    string? StringValue { get; init; }
    DateOnly? DateValue { get; init; }
    double? NumberValue { get; init; }
    Guid? LibraryValueGuid { get; init; }
    DateTime LastUpdated { get; init; }
}
