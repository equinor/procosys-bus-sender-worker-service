using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class LibraryFieldEvent : ILibraryFieldEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid LibraryGuid { get; set; }
    public string? LibraryType { get; set; }
    public string Code { get; set; }
    public string ColumnName { get; set; }
    public string? ColumnType { get; set; }
    public string? StringValue { get; set; }
    public DateOnly? DateValue { get; set; }
    public double? NumberValue { get; set; }
    public Guid? LibraryValueGuid { get; set; }
    public DateTime LastUpdated { get; set; }
    public string EventType => PcsEventConstants.LibraryFieldCreateOrUpdate;
}
