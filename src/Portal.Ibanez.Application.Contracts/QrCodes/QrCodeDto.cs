using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.QrCodes;

public class QrCodeDto : FullAuditedEntityDto<Guid>
{
    public Guid MachineId { get; set; }
    public Guid? DocumentFolderId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}