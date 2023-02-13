﻿namespace Equinor.ProCoSys.PcsServiceBus;


/// <summary>
/// This class defines a set of constants that represent different types of message events that can be used in the event publish-subscribe pattern.
/// </summary>
public static class PcsEventConstants
{

    /// <summary>
    /// Event that is fired when a new checklist is created or updated
    /// </summary>
    public const string ChecklistCreateOrUpdate = "checklistCreatedOrUpdated";


    /// <summary>
    /// Event that is fired when a milestone is created or updated
    /// </summary>
    public const string MilestoneCreateOrUpdate = "milestoneCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a milestone is created or updated
    /// </summary>
    public const string CommPkgMilestoneCreateOrUpdate = "commpkgMilestoneCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new action is created or updated
    /// </summary>
    public const string ActionCreateOrUpdate = "actionCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new CommPkgTask is created or updated
    /// </summary>
    public const string CommPkgTaskCreateOrUpdate = "commPkgTaskCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new task is created or updated
    /// </summary>
    public const string TaskCreateOrUpdate = "taskCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new CommPkg is created or updated
    /// </summary>
    public const string CommPkgCreateOrUpdate = "commPkgCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new Ipo is created or updated
    /// </summary>
    public const string IpoCreateOrUpdate = "ipoCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new McPkg is created or updated
    /// </summary>
    public const string McPkgCreateOrUpdate = "mcPkgCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new project is created or updated
    /// </summary>
    public const string ProjectCreateOrUpdate = "projectCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new responsible is created or updated
    /// </summary>
    public const string ResponsibleCreateOrUpdate = "responsibleCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new tag is created or updated
    /// </summary>
    public const string TagCreateOrUpdate = "tagCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new TagFunction is created or updated
    /// </summary>
    public const string TagFunctionCreateOrUpdate = "tagFunctionCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new PunchListItem is created or updated
    /// </summary>
    public const string PunchListItemCreateOrUpdate = "punchListItemCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new Library is created or updated
    /// </summary>
    public const string LibraryCreateOrUpdate = "libraryCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new WorkOrder is created or updated
    /// </summary>
    public const string WorkOrderCreateOrUpdate = "workOrderCreatedOrUpdated";

    /// <summary>
    /// Event that is fired when a new WorkOrderCutoff is created or updated
    /// </summary>
    public const string WorkOrderCutoffCreateOrUpdate = "workOrderCutoffCreatedOrUpdated";



}
