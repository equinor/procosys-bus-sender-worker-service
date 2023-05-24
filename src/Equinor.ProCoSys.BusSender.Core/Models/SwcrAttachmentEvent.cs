using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class SwcrAttachmentEvent : IAttachmentEventV1
{
    public string ClassificationCode { get; init; }
    public string FileName { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public Guid? SwcrGuid { get; init; }
    public string Title { get; init; }
    public string Uri { get; init; }

    public string EventType => PcsEventConstants.SwcrAttachmentCreateOrUpdate;
}
