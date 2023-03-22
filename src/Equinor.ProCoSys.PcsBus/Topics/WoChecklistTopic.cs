﻿namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class WoChecklistTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string ChecklistId { get; set; }

    public const string TopicName = "wochecklist";
}
