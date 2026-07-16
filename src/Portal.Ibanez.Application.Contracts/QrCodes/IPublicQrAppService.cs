using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.QrCodes;

public interface IPublicQrAppService : IApplicationService
{
    Task<PublicQrExplorerDto?> GetExplorerAsync(
        string code,
        Guid? folderId
    );

    Task<bool> CanDownloadAsync(
        string code,
        Guid documentId
    );
}