﻿namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class TagFunctionTopic
    {
        public string Plant { get; set; }
        public string Code { get; set; }
        public string CodeOld { get; set; }
        public string RegisterCode { get; set; }
        public string RegisterCodeOld { get; set; }
        public string Description { get; set; }
        public bool IsVoided { get; set; }
        public const string TopicName = "tagfunction";
    }
}
