using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Documents;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineDocuments;
[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateMachineDocumentDto Document { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }
    
    [BindProperty]
    public IFormFile PdfFile { get; set; }

    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly IMachineDocumentAppService _machineDocumentAppService;

    public CreateModalModel(
     IMachineDocumentAppService machineDocumentAppService,
     IWebHostEnvironment webHostEnvironment)
    {
        _machineDocumentAppService = machineDocumentAppService;
        _webHostEnvironment = webHostEnvironment;
    }

    public void OnGet()
    {
        Document = new CreateUpdateMachineDocumentDto
        {
            IsActive = true,
            Version = 1,
            ContentType = "application/pdf",
            FileSize = 0,
            StoredFileName = ""
        };
        if (MachineId.HasValue)
        {
            Document.MachineId = MachineId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (PdfFile == null || PdfFile.Length == 0)
        {
            ModelState.AddModelError(nameof(PdfFile), "Debes seleccionar un archivo PDF.");
            return Page();
        }

        if (!PdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(PdfFile), "Solo se permiten archivos PDF.");
            return Page();
        }

        var uploadsRootPath = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "machines",
            Document.MachineId.ToString()
        );

        Directory.CreateDirectory(uploadsRootPath);

        var storedFileName = $"{Guid.NewGuid():N}.pdf";
        var fullPath = Path.Combine(uploadsRootPath, storedFileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await PdfFile.CopyToAsync(stream);
        }

        Document.FileName = PdfFile.FileName;
        Document.StoredFileName = storedFileName;
        Document.ContentType = PdfFile.ContentType;
        Document.FileSize = PdfFile.Length;
        Document.Version = 1;
        Document.IsActive = true;

        await _machineDocumentAppService.CreateAsync(Document);

        return NoContent();
    }
}