﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgEvent : ICommPkgEventV1
{
    public string? AreaCode { get; init; }
    public string? AreaDescription { get; init; }
    public string? CommissioningIdentifier { get; init; }
    public long CommPkgId { get; init; }
    public string CommPkgNo { get; init; }
    public string CommPkgStatus { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? DCCommPkgStatus { get; init; }
    public bool? Demolition { get; init; }
    public string Description { get; init; }
    public string? DescriptionOfWork { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public string? Phase { get; init; }
    public string Plant { get; init; }
    public string PlantName { get; init; }
    public string? Priority1 { get; init; }
    public string? Priority2 { get; init; }
    public string? Priority3 { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string? Progress { get; init; }
    public string ProjectName { get; init; }
    public string? Remark { get; init; }
    public string ResponsibleCode { get; init; }
    public string? ResponsibleDescription { get; init; }

    public string EventType => PcsEventConstants.CommPkgCreateOrUpdate;
}
