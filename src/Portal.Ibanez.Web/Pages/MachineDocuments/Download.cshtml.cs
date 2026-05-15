using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Documents;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineDocuments;

[Authorize]
public class DownloadModel : IbanezPageModel
{
    private readonly IMachineDocumentAppService _machineDocumentAppService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DownloadModel(
        IMachineDocumentAppService machineDocumentAppService,
        IWebHostEnvironment webHostEnvironment)
    {
        _machineDocumentAppService = machineDocumentAppService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var document = await _machineDocumentAppService.GetAsync(id);

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