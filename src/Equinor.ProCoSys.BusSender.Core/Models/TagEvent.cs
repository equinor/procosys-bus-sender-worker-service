﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class TagEvent : ITagEventV1
{
    public string EventType => PcsEventConstants.TagCreateOrUpdate;
    public string? AreaCode { get; init; }
    public string? AreaDescription { get; init; }
    public Guid? CallOffGuid { get; init; }
    public string? CallOffNo { get; init; }
    public Guid? CommPkgGuid { get; init; }
    public string? CommPkgNo { get; init; }
    public string? Description { get; init; }
    public string? DisciplineCode { get; init; }
    public string? DisciplineDescription { get; init; }
    public string? EngineeringCode { get; init; }
    public string? InstallationCode { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public Guid? McPkgGuid { get; init; }
    public string? McPkgNo { get; init; }
    public int? MountedOn { get; init; }
    public Guid? MountedOnGuid { get; init; }

    public string Plant { get; init; }
    public string PlantName { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string? ProjectName { get; init; }
    public string? PurchaseOrderNo { get; init; }
    public string? RegisterCode { get; init; }
    public string? Status { get; init; }
    public string? System { get; init; }

    public string? TagDetails { get; init; }
    public string? TagFunctionCode { get; init; }
    public long TagId { get; init; }
    public string TagNo { get; init; }
}
