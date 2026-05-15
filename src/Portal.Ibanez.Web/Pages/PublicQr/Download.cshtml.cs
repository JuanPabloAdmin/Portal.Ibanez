using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Documents;
using Portal.Ibanez.QrCodes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.PublicQr;

[AllowAnonymous]
public class DownloadModel : IbanezPageModel
{
    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IMachineDocumentAppService _machineDocumentAppService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DownloadModel(
        IQrCodeAppService qrCodeAppService,
        IMachineDocumentAppService machineDocumentAppService,
        IWebHostEnvironment webHostEnvironment)
    {
        _qrCodeAppService = qrCodeAppService;
        _machineDocumentAppService = machineDocumentAppService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(string code, Guid documentId)
    {
        var qrCode = await _qrCodeAppService.GetByCodeAsync(code);

        if (qrCode == null || !qrCode.IsActive)
        {
            return NotFound();
        }

        var associatedDocuments = await _qrCodeAppService.GetDocumentsAsync(qrCode.Id);

        var isAssociated = associatedDocuments.Any(x => x.MachineDocumentId == documentId);

        if (!isAssociated)
        {
            return NotFound();
        }

        var document = await _machineDocumentAppService.GetAsync(documentId);

        if (!document.IsActive)
        {
            return NotFound();
        }

        var fullPath = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "machines",
            document.MachineId.ToString(),
            document.StoredFileName
        );

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

        return File(fileBytes, document.ContentType, document.FileName);
    }
}