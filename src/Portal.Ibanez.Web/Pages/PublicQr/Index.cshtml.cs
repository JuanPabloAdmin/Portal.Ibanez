using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.PublicQr;

[AllowAnonymous]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public string Code { get; set; }

    public string Message { get; set; }

    public List<QrCodeDocumentDto> Documents { get; set; } = new();

    private readonly IQrCodeAppService _qrCodeAppService;

    public IndexModel(IQrCodeAppService qrCodeAppService)
    {
        _qrCodeAppService = qrCodeAppService;
    }

    public async Task<IActionResult> OnGetAsync(string code)
    {
        Code = code;

        var qrCode = await _qrCodeAppService.GetByCodeAsync(code);

        if (qrCode == null || !qrCode.IsActive)
        {
            Message = "El código QR no está disponible.";
            return Page();
        }

        Documents = await _qrCodeAppService.GetDocumentsAsync(qrCode.Id);

        if (Documents.Count == 0)
        {
            Message = "No hay documentos asociados a este código QR.";
            return Page();
        }

        if (Documents.Count == 1)
        {
            return Redirect($"/q/{Code}/download/{Documents[0].MachineDocumentId}");
        }

        return Page();
    }
}