using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.Machines;

public interface IMachineAppService :
    ICrudAppService<
        MachineDto,
        Guid,
        GetMachineListInput,
        CreateUpdateMachineDto>
{
    Task<DuplicateMachineResultDto> DuplicateAsync(
      Guid sourceMachineId,
      DuplicateMachineDto input
  );
}