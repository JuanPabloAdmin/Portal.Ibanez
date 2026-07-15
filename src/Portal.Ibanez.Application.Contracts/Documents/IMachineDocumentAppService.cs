using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Portal.Ibanez.Documents;

public interface IMachineDocumentAppService :
    ICrudAppService<
        MachineDocumentDto,
        Guid,
    GetMachineDocumentListInput,
        CreateUpdateMachineDocumentDto>
{
    Task<List<MachineDocumentDto>> GetByFolderAsync(Guid documentFolderId);
}