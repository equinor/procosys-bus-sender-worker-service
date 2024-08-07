﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;


namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

#pragma warning disable CS8618
// ReSharper disable once ClassNeverInstantiated.Global
public class PersonEvent : IPersonEventV1
{
    public string EventType => PcsEventConstants.PersonUpdate;
    public Guid ProCoSysGuid { get; init; }
    public Guid? AzureOid { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public bool SuperUser { get; init; }
    public DateTime LastUpdated { get; init; }
}
