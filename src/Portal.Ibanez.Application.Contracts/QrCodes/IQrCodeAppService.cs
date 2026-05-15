using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.QrCodes;

public interface IQrCodeAppService :
    ICrudAppService<
        QrCodeDto,
        Guid,
        GetQrCodeListInput,
        CreateUpdateQrCodeDto>
{
    Task<List<QrCodeDocumentDto>> GetDocumentsAsync(Guid qrCodeId);

    Task AddDocumentAsync(AddQrCodeDocumentDto input);

    Task RemoveDocumentAsync(Guid qrCodeDocumentId);
    Task<QrCodeDto?> GetByCodeAsync(string code);
}