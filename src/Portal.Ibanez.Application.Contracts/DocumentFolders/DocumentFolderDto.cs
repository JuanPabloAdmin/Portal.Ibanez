using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.DocumentFolders;

public class DocumentFolderDto : FullAuditedEntityDto<Guid>
{
    public Guid MachineId { get; set; }
    public Guid? ParentFolderId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}