using System;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.DocumentFolders;

public interface IDocumentFolderAppService :
    ICrudAppService<
        DocumentFolderDto,
        Guid,
        GetDocumentFolderListInput,
        CreateUpdateDocumentFolderDto>
{
}