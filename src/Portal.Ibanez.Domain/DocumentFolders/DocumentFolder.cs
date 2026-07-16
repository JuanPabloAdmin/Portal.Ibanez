using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.DocumentFolders;

public class DocumentFolder : FullAuditedAggregateRoot<Guid>
{
    public Guid MachineId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    protected DocumentFolder()
    {
    }

    public DocumentFolder(
        Guid id,
        Guid machineId,
        string name
    ) : base(id)
    {
        MachineId = machineId;
        Name = name;
        IsActive = true;
    }
}