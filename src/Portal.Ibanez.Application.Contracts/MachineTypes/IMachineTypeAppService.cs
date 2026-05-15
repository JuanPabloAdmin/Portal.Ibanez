using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.MachineTypes;

public interface IMachineTypeAppService :
    ICrudAppService<
        MachineTypeDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateMachineTypeDto>
{
}