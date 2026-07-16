using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.DocumentFolders;

namespace Portal.Ibanez.Web.Pages.DocumentFolders;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateDocumentFolderDto Folder { get; set; } = new();

    private readonly IDocumentFolderAppService _documentFolderAppService;

    public CreateModalModel(
        IDocumentFolderAppService documentFolderAppService)
    {
        _documentFolderAppService = documentFolderAppService;
    }

    public void OnGet(Guid machineId, Guid? parentFolderId)
    {
        Folder = new CreateUpdateDocumentFolderDto
        {
            MachineId = machineId,
            ParentFolderId = parentFolderId,
            IsActive = true
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Folder.MachineId == Guid.Empty)
        {
            ModelState.AddModelError(
                "Folder.MachineId",
                "No se recibió correctamente la máquina."
            );

            return Page();
        }

        await _documentFolderAppService.CreateAsync(Folder);

        return NoContent();
    }
}