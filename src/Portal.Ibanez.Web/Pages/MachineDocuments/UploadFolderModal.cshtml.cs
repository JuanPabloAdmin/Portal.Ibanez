using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Documents;

namespace Portal.Ibanez.Web.Pages.MachineDocuments;

[Authorize]
public class UploadFolderModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid MachineId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid DocumentFolderId { get; set; }

    [BindProperty]
    public List<IFormFile> FolderFiles { get; set; } = new();

    private readonly IMachineDocumentAppService _machineDocumentAppService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UploadFolderModalModel(
        IMachineDocumentAppService machineDocumentAppService,
        IWebHostEnvironment webHostEnvironment)
    {
        _machineDocumentAppService = machineDocumentAppService;
        _webHostEnvironment = webHostEnvironment;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (MachineId == Guid.Empty)
        {
            ModelState.AddModelError(
                nameof(MachineId),
                "No se ha indicado la máquina."
            );
        }

        if (DocumentFolderId == Guid.Empty)
        {
            ModelState.AddModelError(
                nameof(DocumentFolderId),
                "No se ha indicado la carpeta documental."
            );
        }

        var pdfFiles = FolderFiles
            .Where(IsPdf)
            .ToList();

        if (pdfFiles.Count == 0)
        {
            ModelState.AddModelError(
                nameof(FolderFiles),
                "La carpeta seleccionada no contiene archivos PDF."
            );
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var uploadsRootPath = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "machines",
            MachineId.ToString(),
            DocumentFolderId.ToString()
        );

        Directory.CreateDirectory(uploadsRootPath);

        var savedFiles = new List<string>();

        try
        {
            foreach (var pdfFile in pdfFiles)
            {
                var originalFileName = Path.GetFileName(pdfFile.FileName);
                var relativePath = NormalizeRelativePath(pdfFile.FileName);

                var storedFileName = $"{Guid.NewGuid():N}.pdf";
                var fullPath = Path.Combine(uploadsRootPath, storedFileName);

                await using (var stream = new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                savedFiles.Add(fullPath);

                var document = new CreateUpdateMachineDocumentDto
                {
                    MachineId = MachineId,
                    DocumentFolderId = DocumentFolderId,
                    Title = Path.GetFileNameWithoutExtension(originalFileName),
                    FileName = originalFileName,
                    StoredFileName = storedFileName,
                    RelativePath = relativePath,
                    ContentType = "application/pdf",
                    FileSize = pdfFile.Length,
                    Version = 1,
                    IsActive = true
                };

                await _machineDocumentAppService.CreateAsync(document);
            }
        }
        catch
        {
            foreach (var savedFile in savedFiles)
            {
                if (System.IO.File.Exists(savedFile))
                {
                    System.IO.File.Delete(savedFile);
                }
            }

            throw;
        }

        return NoContent();
    }

    private static bool IsPdf(IFormFile file)
    {
        if (file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName);

        return extension.Equals(
                   ".pdf",
                   StringComparison.OrdinalIgnoreCase
               ) ||
               file.ContentType.Equals(
                   "application/pdf",
                   StringComparison.OrdinalIgnoreCase
               );
    }

    private static string NormalizeRelativePath(string fileName)
    {
        var normalized = fileName.Replace('\\', '/');

        var segments = normalized
            .Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries
            )
            .Where(segment =>
                segment != "." &&
                segment != "..")
            .ToArray();

        return string.Join('/', segments);
    }
}