using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class TagTopic
    {
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string TagId { get; set; }
        public string TagNo { get; set; }
        public string TagNoOld { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNameOld { get; set; }
        public string McPkgNo { get; set; }
        public string McPkgNoOld { get; set; }
        public string CommPkgNo { get; set; }
        public string CommPkgNoOld { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string DisciplineCode { get; set; }
        public string DisciplineDescription { get; set; }
        public string RegisterCode { get; set; }
        public string Status { get; set; }
        public string System { get; set; }
        public string CallOffNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagDetails { get; set; }
        public List<string> ProjectNames { get; set; }
        public string LastUpdated { get; set; }
        public const string TopicName = "tag";
    }
}
