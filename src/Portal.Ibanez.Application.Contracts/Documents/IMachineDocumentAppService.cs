using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.Documents;

public interface IMachineDocumentAppService :
    ICrudAppService<
        MachineDocumentDto,
        Guid,
    GetMachineDocumentListInput,
        CreateUpdateMachineDocumentDto>
{
}