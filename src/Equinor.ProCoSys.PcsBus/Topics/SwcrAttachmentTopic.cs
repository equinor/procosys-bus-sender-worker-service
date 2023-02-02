using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class SwcrAttachmentTopic
{
    public string Plant { get; set; }
    public string ProCoSysGuid { get; set; }
    public string SwcrGuid { get; set; }
    public string Title { get; set; }
    public string ClassificationCode { get; set; }
    public string URI { get; set; }
    public string FileName { get; set; }

    public const string TopicName = "swcrattachment";
}
