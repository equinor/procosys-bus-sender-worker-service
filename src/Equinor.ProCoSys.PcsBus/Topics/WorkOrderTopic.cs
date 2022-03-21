﻿using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class WorkOrderTopic
    {
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string ProjectName { get; set; }
        public string WoId { get; set; }
        public string WoNo { get; set; }
        public string WoNoOld { get; set; }
        public string CommPkgNo { get; set; }
        public string CommPkgNoOld { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MileStoneCode { get; set; }
        public string MilestoneDescription { get; set; }
        public string MaterialStatusCode { get; set; }
        public string CategoryCode { get; set; }
        public string HoldByCode { get; set; }
        public string ResponsibleCode { get; set; }
        public string ResponsibleDescription { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string JobStatusCode { get; set; }
        public string MaterialComments { get; set; }
        public string ConstructionComments { get; set; }
        public string TypeOfWorkCode { get; set; }
        public string OnShoreOffShoreCode { get; set; }
        public string WoTypeCode { get; set; }
        public string DisciplineCode { get; set; }
        public string DisciplineDescription { get; set; }
        public string ProjectProgress { get; set; }
        public string ExpendedManHours { get; set; }
        public string EstimatedHours { get; set; }
        public string RemainingHours { get; set; }
        public DateTime PlannedStartAtDate { get; set; }
        public DateTime ActualStartAtDate { get; set; }
        public DateTime PlannedFinishedAtDate { get; set; }
        public DateTime ActualFinishedAtDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVoided { get; set; }
        public DateTime LastUpdated { get; set; }

        public const string TopicName = "wo";
    }
}   
