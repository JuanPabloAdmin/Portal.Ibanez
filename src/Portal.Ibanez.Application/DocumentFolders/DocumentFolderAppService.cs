using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.DocumentFolders;

public class DocumentFolderAppService :
    CrudAppService<
        DocumentFolder,
        DocumentFolderDto,
        Guid,
        GetDocumentFolderListInput,
        CreateUpdateDocumentFolderDto>,
    IDocumentFolderAppService
{
    public DocumentFolderAppService(
        IRepository<DocumentFolder, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<PagedResultDto<DocumentFolderDto>> GetListAsync(
        GetDocumentFolderListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.MachineId.HasValue)
        {
            queryable = queryable.Where(
                x => x.MachineId == input.MachineId.Value
            );
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var entities = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? "Name")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<DocumentFolderDto>(
            totalCount,
            ObjectMapper.Map<
                List<DocumentFolder>,
                List<DocumentFolderDto>
            >(entities)
        );
    }
}