using Equinor.ProCoSys.MessageContracts.Punch;

namespace Equinor.ProCoSys.BusReceiver.Events;

public record PunchCreatedMessage
(
     string DisplayName,
     Guid ProjectGuid ,
     Guid Guid ,
     string ItemNo ,
     Guid CreatedByOid,
     DateTime CreatedAtUtc
) : IPunchCreatedV1;
