﻿using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class PunchListItemTopic
{
    public string Plant { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string ProCoSysGuid { get; set; }
    public List<string> ProjectNames { get; set; }
    public string LastUpdated { get; set; }
    public string PunchItemNo { get; set; }
    public string Description { get; set; }
    public string TagNo { get; set; }
    public string ResponsibleCode { get; set; }
    public string ResponsibleDescription { get; set; }
    public string FormType { get; set; }
    public string Category { get; set; }
    public const string TopicName = "punchlistitem";
}
