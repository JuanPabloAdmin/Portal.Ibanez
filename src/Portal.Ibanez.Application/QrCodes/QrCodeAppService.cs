using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace Portal.Ibanez.QrCodes;


public class QrCodeAppService :
    CrudAppService<
        QrCode,
        QrCodeDto,
        Guid,
        GetQrCodeListInput,
        CreateUpdateQrCodeDto>,
    IQrCodeAppService
{


    public QrCodeAppService(
      IRepository<QrCode, Guid> repository,
   
      IRepository<MachineDocument, Guid> machineDocumentRepository)
      : base(repository)
    {

    }

    public override async Task<PagedResultDto<QrCodeDto>> GetListAsync(GetQrCodeListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.MachineId.HasValue)
        {
            queryable = queryable.Where(x => x.MachineId == input.MachineId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? "Name")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<QrCodeDto>(
            totalCount,
            ObjectMapper.Map<List<QrCode>, List<QrCodeDto>>(items)
        );
    }

    public async Task<QrCodeDto?> GetByCodeAsync(string code)
    {
        var queryable = await Repository.GetQueryableAsync();

        var entity = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.Code == code)
        );

        if (entity == null)
        {
            return null;
        }

        return ObjectMapper.Map<QrCode, QrCodeDto>(entity);
    }
}