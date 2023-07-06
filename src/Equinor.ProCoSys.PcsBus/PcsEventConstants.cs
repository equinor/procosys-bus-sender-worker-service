namespace Equinor.ProCoSys.PcsServiceBus;

/// <summary>
///     This class defines a set of constants that represent different types of message events that can be used in the
///     event publish-subscribe pattern.
/// </summary>
public static class PcsEventConstants
{
    /// <summary>
    ///     Event that is fired when a new checklist is created or updated
    /// </summary>
    public const string ChecklistCreateOrUpdate = "checklistCreatedOrUpdated";


    /// <summary>
    ///     Event that is fired when a milestone is created or updated
    /// </summary>
    public const string MilestoneCreateOrUpdate = "milestoneCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a milestone is created or updated
    /// </summary>
    public const string CommPkgMilestoneCreateOrUpdate = "commpkgMilestoneCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new action is created or updated
    /// </summary>
    public const string ActionCreateOrUpdate = "actionCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new CommPkgTask is created or updated
    /// </summary>
    public const string CommPkgTaskCreateOrUpdate = "commPkgTaskCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new task is created or updated
    /// </summary>
    public const string TaskCreateOrUpdate = "taskCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new CommPkg is created or updated
    /// </summary>
    public const string CommPkgCreateOrUpdate = "commPkgCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new Ipo is created or updated
    /// </summary>
    public const string IpoCreateOrUpdate = "ipoCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new McPkg is created or updated
    /// </summary>
    public const string McPkgCreateOrUpdate = "mcPkgCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new project is created or updated
    /// </summary>
    public const string ProjectCreateOrUpdate = "projectCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new responsible is created or updated
    /// </summary>
    public const string ResponsibleCreateOrUpdate = "responsibleCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new tag is created or updated
    /// </summary>
    public const string TagCreateOrUpdate = "tagCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new TagFunction is created or updated
    /// </summary>
    public const string TagFunctionCreateOrUpdate = "tagFunctionCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new PunchListItem is created or updated
    /// </summary>
    public const string PunchListItemCreateOrUpdate = "punchListItemCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new Library is created or updated
    /// </summary>
    public const string LibraryCreateOrUpdate = "libraryCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new WorkOrder is created or updated
    /// </summary>
    public const string WorkOrderCreateOrUpdate = "workOrderCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new WorkOrderCutoff is created or updated
    /// </summary>
    public const string WorkOrderCutoffCreateOrUpdate = "workOrderCutoffCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new WorkOrderMilestone is created or updated
    /// </summary>
    public const string WorkOrderMilestoneCreateOrUpdate = "workOrderCutoffCreatedOrUpdated";

    public const string ChecklistDelete = "checklistDeleted";

    /// <summary>
    ///     Event that is fired when a new CalOff is created or updated
    /// </summary>
    public const string CallOffCreateOrUpdate = "callOffCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new CommPkgOperation is created or updated
    /// </summary>
    public const string CommPkgOperationCreateOrUpdate = "commPkgOperationCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new CommPkg_Query (connection between commPackage and query) is created or updated
    /// </summary>
    public const string CommPkgQueryCreateOrUpdate = "commPkgQueryCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new HeatTrace is created or updated
    /// </summary>
    public const string HeatTraceCreateOrUpdate = "heatTraceCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new LibraryField is created or updated
    /// </summary>
    public const string LibraryFieldCreateOrUpdate = "libraryFieldCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new swcr is created or updated
    /// </summary>
    public const string SwcrCreateOrUpdate = "swcrCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new piping revision is created or updated
    /// </summary>
    public const string PipingRevisionCreateOrUpdate = "pipingRevisionCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new piping spool is created or updated
    /// </summary>
    public const string PipingSpoolCreateOrUpdate = "pipingSpoolCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new document is created or updated
    /// </summary>
    public const string DocumentCreateOrUpdate = "documentCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new loop tag is created or updated
    /// </summary>
    public const string LoopTagCreateOrUpdate = "loopTagCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new query is created or updated
    /// </summary>
    public const string QueryCreateOrUpdate = "queryCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new query signature is created or updated
    /// </summary>
    public const string QuerySignatureCreateOrUpdate = "querySignatureCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new stock is created or updated
    /// </summary>
    public const string StockCreateOrUpdate = "stockCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new swcrOtherReference is created or updated
    /// </summary>
    public const string SwcrOtherReferenceCreateOrUpdate = "swcrOtherReferenceCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new swcr attachment is created or updated
    /// </summary>
    public const string SwcrAttachmentCreateOrUpdate = "swcrAttachmentCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new swcr signature is created or updated
    /// </summary>
    public const string SwcrSignatureCreateOrUpdate = "swcrSignatureCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new swcr type is created or updated
    /// </summary>
    public const string SwcrTypeCreateOrUpdate = "swcrTypeCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new workorderChecklist is created or updated
    /// </summary>
    public const string WorkOrderChecklistCreateOrUpdate = "workOrderChecklistCreatedOrUpdated";


    /// <summary>
    ///     Event that is fired when a new workorder material is created or updated
    /// </summary>
    public const string WorkOrderMaterialCreateOrUpdate = "workOrderMaterialCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new tag equipment  is created or updated
    /// </summary>
    public const string TagEquipmentCreateOrUpdate = "tagEquipmentCreatedOrUpdated";

    /// <summary>
    ///     Event that is fired when a new heat trace pipe test is created or updated
    /// </summary>
    public const string HeatTracePipeTestEventType = "heatTracePipeTestCreatedOrUpdated";
}
