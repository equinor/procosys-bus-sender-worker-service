﻿namespace Equinor.ProCoSys.PcsServiceBus;

public enum PcsTopic
{
    Action,
    CommPkgTask,
    Task,
    CommPkg,
    Ipo,
    McPkg,
    Project,
    Responsible,
    Tag,
    TagFunction,
    PunchListItem,
    Library,
    WorkOrder,
    Checklist,
    Milestone,
    WorkOrderCutoff,
    // todo Certificate Topic is used for "accept of a certificate" (consumed in preservation).
    // Can't be reused as-is for "create certificate" or "edit certificate". See #98630
    Certificate,
    WoChecklist,
    SWCR,
    SWCRSignature,
    SWCRType,
    SWCROtherReference,
    PipingRevision,
    WoMaterial,
    WoMilestone,
    Stock,
    Document,
    CommPkgOperation,
    PipingSpool,
    LoopContent,
    Query,
    QuerySignature,
    CallOff,
    CommPkgQuery,
    HeatTrace
}
