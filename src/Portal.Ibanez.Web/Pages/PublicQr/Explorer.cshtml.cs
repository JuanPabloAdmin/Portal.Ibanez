using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;

namespace Portal.Ibanez.Web.Pages.PublicQr;

[AllowAnonymous]
public class ExplorerModel : IbanezPageModel
{
    private readonly IPublicQrAppService _publicQrAppService;

    public ExplorerModel(IPublicQrAppService publicQrAppService)
    {
        _publicQrAppService = publicQrAppService;
    }

    public async Task<IActionResult> OnGetAsync(
        string code,
        Guid? folderId)
    {
        var result = await _publicQrAppService.GetExplorerAsync(
            code,
            folderId
        );

        if (result == null)
        {
            return NotFound(new
            {
                message = "El código QR o la carpeta solicitada no son válidos."
            });
        }

        return new JsonResult(result);
    }
}