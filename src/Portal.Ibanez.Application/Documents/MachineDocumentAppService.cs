using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.Documents;

[Authorize]
public class MachineDocumentAppService :
    CrudAppService<
        MachineDocument,
        MachineDocumentDto,
        Guid,
      GetMachineDocumentListInput,
        CreateUpdateMachineDocumentDto>,
    IMachineDocumentAppService
{
    public MachineDocumentAppService(IRepository<MachineDocument, Guid> repository)
        : base(repository)
    {
    }
    public override async Task<PagedResultDto<MachineDocumentDto>> GetListAsync(GetMachineDocumentListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.MachineId.HasValue)
        {
            queryable = queryable.Where(x => x.MachineId == input.MachineId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? "Title")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<MachineDocumentDto>(
            totalCount,
            ObjectMapper.Map<List<MachineDocument>, List<MachineDocumentDto>>(items)
        );
    }
}