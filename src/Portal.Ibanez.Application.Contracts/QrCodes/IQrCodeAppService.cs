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
   
    Task<QrCodeDto?> GetByCodeAsync(string code);
}