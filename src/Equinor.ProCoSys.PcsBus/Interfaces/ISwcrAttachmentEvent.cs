using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

/// <summary>
///  V1 of this interface is only used for SWCR, and swcr fields will always be populated.
/// However when/if we role out V2 its very likely it will be a for all attachments, and swcr fields will be optional.
/// Therefor we keep them as nullable, even if they should always have a value.
/// </summary>
public interface IAttachmentEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid? SwcrGuid { get; set; }
    string Title { get; set; }  
    string ClassificationCode { get; set; }
    string URI { get; set; }
    string FileName { get; set; }
    DateTime LastUpdated { get; set; }
    
}
