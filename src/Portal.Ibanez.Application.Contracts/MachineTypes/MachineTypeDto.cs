using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.MachineTypes;

public class MachineTypeDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
}