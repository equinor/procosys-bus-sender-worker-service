using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderEventV1 : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    string ProjectName { get; }
    string WoNo { get;  }
    long WoId { get; }
    string? CommPkgNo { get; }
    Guid? CommPkgGuid { get; }
    string? Title { get; }
    string? Description { get; }
    string? MilestoneCode { get; }
    string? SubMilestoneCode { get; }
    string? MilestoneDescription { get;  }
    string? CategoryCode { get;  }
    string? MaterialStatusCode { get;  }
    string? HoldByCode { get;  }
    string? DisciplineCode { get;  }
    string? DisciplineDescription { get;  }
    string? ResponsibleCode { get;  }
    string? ResponsibleDescription { get;  }
    string? AreaCode { get;  }
    string? AreaDescription { get;  }
    string? JobStatusCode { get;  }
    string? MaterialComments { get;  }
    string? ConstructionComments { get;  }
    string? TypeOfWorkCode { get;  }
    string? OnShoreOffShoreCode { get;  }
    string? WoTypeCode { get;  }
    string? ProjectProgress { get;  }
    string? ExpendedManHours { get;  }
    string? EstimatedHours { get;  }
    string? RemainingHours { get;  }
    int Progress { get; }
    DateOnly? PlannedStartAtDate { get;  }
    DateOnly? ActualStartAtDate { get;  }
    DateOnly? PlannedFinishedAtDate { get;  }
    DateOnly? ActualFinishedAtDate { get;  }
    DateTime CreatedAt { get;  }
    bool IsVoided { get;  }
    DateTime LastUpdated { get;  }
}
