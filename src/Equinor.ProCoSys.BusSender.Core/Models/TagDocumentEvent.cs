﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public sealed record TagDocumentEvent(
    string Plant,
    Guid ProCoSysGuid,
    long TagId,
    Guid TagGuid,
    long DocumentId,
    Guid DocumentGuid,
    DateTime LastUpdated,
    string EventType = PcsEventConstants.TagDocumentCreateOrUpdate
    ) : ITagDocumentEventV1;
