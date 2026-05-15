using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Documents;

public class MachineDocumentDto : FullAuditedEntityDto<Guid>
{
    public Guid MachineId { get; set; }

    public string Title { get; set; }

    public string FileName { get; set; }

    public string StoredFileName { get; set; }

    public string ContentType { get; set; }

    public long FileSize { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }
}