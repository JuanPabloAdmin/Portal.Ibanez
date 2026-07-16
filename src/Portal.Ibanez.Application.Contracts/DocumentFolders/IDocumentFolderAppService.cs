using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Portal.Ibanez.DocumentFolders;

public interface IDocumentFolderAppService :
    ICrudAppService<
        DocumentFolderDto,
        Guid,
        GetDocumentFolderListInput,
        CreateUpdateDocumentFolderDto>
{
    Task<FolderExplorerDto> GetExplorerAsync( GetFolderExplorerInput input);

    Task<FolderContentStateDto> GetContentStateAsync(Guid folderId);
}