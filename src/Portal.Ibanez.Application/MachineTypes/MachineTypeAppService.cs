using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Machines;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.MachineTypes;

[Authorize]
public class MachineTypeAppService :
    CrudAppService<
        MachineType,
        MachineTypeDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateMachineTypeDto>,
    IMachineTypeAppService
{
    public MachineTypeAppService(IRepository<MachineType, Guid> repository)
        : base(repository)
    {
    }
}