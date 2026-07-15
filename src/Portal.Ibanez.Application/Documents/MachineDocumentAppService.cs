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
        if (input.DocumentFolderId.HasValue)
        {
            queryable = queryable.Where(
                x => x.DocumentFolderId == input.DocumentFolderId.Value
            );
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

    // SOBRESCRITURA PARA MANEJO AUTOMÁTICO DE VERSIONES
    public override async Task<MachineDocumentDto> UpdateAsync(Guid id, CreateUpdateMachineDocumentDto input)
    {
        var document = await Repository.GetAsync(id);

        // Lógica: Si el nombre del archivo guardado cambia, asumimos que es una versión nueva
        if (document.StoredFileName != input.StoredFileName && !string.IsNullOrEmpty(input.StoredFileName))
        {
            input.Version = document.Version + 1;
        }
        else
        {
            input.Version = document.Version; // Mantenemos la versión si solo se edita el título
        }

        return await base.UpdateAsync(id, input);
    }

    // MÉTODO PARA ACTIVAR/DESACTIVAR RÁPIDAMENTE
    public async Task ToggleActiveAsync(Guid id)
    {
        var document = await Repository.GetAsync(id);
        document.IsActive = !document.IsActive;
        await Repository.UpdateAsync(document);
    }

    public async Task<List<MachineDocumentDto>> GetByFolderAsync(Guid documentFolderId)
    {
        var queryable = await Repository.GetQueryableAsync();

        var query = queryable
            .Where(x =>
                x.DocumentFolderId == documentFolderId &&
                x.IsActive
            )
            .OrderBy(x => x.RelativePath ?? x.FileName);

        var entities = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<
            List<MachineDocument>,
            List<MachineDocumentDto>
        >(entities);
    }
}