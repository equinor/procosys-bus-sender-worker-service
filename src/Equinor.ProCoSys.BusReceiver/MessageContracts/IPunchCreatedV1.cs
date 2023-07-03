// ReSharper disable once CheckNamespace
namespace Equinor.ProCoSys.Completion.MessageContracts.Punch;

public interface IPunchCreatedV1 
{
    public string DisplayName { get;  }
    public Guid ProjectGuid { get; }
    public Guid Guid { get; }
    public string ItemNo { get;  }
    public Guid CreatedByOid { get; }
    public DateTime CreatedAtUtc { get;  }
}
