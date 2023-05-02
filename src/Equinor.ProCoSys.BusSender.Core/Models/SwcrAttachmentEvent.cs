using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class SwcrAttachmentEvent : IAttachmentEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid? SwcrGuid { get; set; }
    public string Title { get; set; }
    public string ClassificationCode { get; set; }
    public string Uri { get; set; }
    public string FileName { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.SwcrAttachmentCreateOrUpdate;
}
