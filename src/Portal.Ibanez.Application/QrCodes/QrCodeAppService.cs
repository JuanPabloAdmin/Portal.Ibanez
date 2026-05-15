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

    private readonly IRepository<QrCodeDocument, Guid> _qrCodeDocumentRepository;
    private readonly IRepository<MachineDocument, Guid> _machineDocumentRepository;
    public QrCodeAppService(
      IRepository<QrCode, Guid> repository,
      IRepository<QrCodeDocument, Guid> qrCodeDocumentRepository,
      IRepository<MachineDocument, Guid> machineDocumentRepository)
      : base(repository)
    {
        _qrCodeDocumentRepository = qrCodeDocumentRepository;
        _machineDocumentRepository = machineDocumentRepository;
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
    public async Task<List<QrCodeDocumentDto>> GetDocumentsAsync(Guid qrCodeId)
    {
        var qrCodeDocuments = await _qrCodeDocumentRepository.GetQueryableAsync();
        var machineDocuments = await _machineDocumentRepository.GetQueryableAsync();

        var query =
            from qrd in qrCodeDocuments
            join doc in machineDocuments on qrd.MachineDocumentId equals doc.Id
            where qrd.QrCodeId == qrCodeId
            orderby qrd.DisplayOrder
            select new QrCodeDocumentDto
            {
                Id = qrd.Id,
                QrCodeId = qrd.QrCodeId,
                MachineDocumentId = qrd.MachineDocumentId,
                DocumentTitle = doc.Title,
                FileName = doc.FileName,
                DisplayOrder = qrd.DisplayOrder
            };

        return await AsyncExecuter.ToListAsync(query);
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
    public async Task AddDocumentAsync(AddQrCodeDocumentDto input)
    {
        var exists = await _qrCodeDocumentRepository.AnyAsync(x =>
            x.QrCodeId == input.QrCodeId &&
            x.MachineDocumentId == input.MachineDocumentId
        );

        if (exists)
        {
            throw new UserFriendlyException("Este documento ya está asociado al QR.");
        }

        var entity = new QrCodeDocument(
            GuidGenerator.Create(),
            input.QrCodeId,
            input.MachineDocumentId
        )
        {
            DisplayOrder = input.DisplayOrder
        };

        await _qrCodeDocumentRepository.InsertAsync(entity);
    }

    public async Task RemoveDocumentAsync(Guid qrCodeDocumentId)
    {
        await _qrCodeDocumentRepository.DeleteAsync(qrCodeDocumentId);
    }
}