using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portal.Ibanez.Documents;
namespace Portal.Ibanez.Web.Pages.PublicQr;

[AllowAnonymous]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public string Code { get; set; }

    public string Message { get; set; }

    public List<MachineDocumentDto> Documents { get; set; } = new();

    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IMachineDocumentAppService _machineDocumentAppService;
    public IndexModel(
     IQrCodeAppService qrCodeAppService,
     IMachineDocumentAppService machineDocumentAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _machineDocumentAppService = machineDocumentAppService;
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

        if (!qrCode.DocumentFolderId.HasValue)
        {
            Message = "Este código QR no tiene una carpeta documental asociada.";
            return Page();
        }

        Documents = await _machineDocumentAppService.GetByFolderAsync(
            qrCode.DocumentFolderId.Value
        );

        if (Documents.Count == 0)
        {
            Message = "No hay documentos asociados a este código QR.";
            return Page();
        }

        if (Documents.Count == 1)
        {
            return Redirect($"/q/{Code}/download/{Documents[0].Id}");
        }

        return Page();
    }
}