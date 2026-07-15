using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.DocumentFolders;

namespace Portal.Ibanez.Web.Pages.DocumentFolders;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    [BindProperty]
    public CreateUpdateDocumentFolderDto Folder { get; set; }

    private readonly IDocumentFolderAppService _documentFolderAppService;

    public CreateModalModel(
        IDocumentFolderAppService documentFolderAppService)
    {
        _documentFolderAppService = documentFolderAppService;
    }

    public void OnGet()
    {
        Folder = new CreateUpdateDocumentFolderDto
        {
            IsActive = true
        };

        if (MachineId.HasValue)
        {
            Folder.MachineId = MachineId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _documentFolderAppService.CreateAsync(Folder);

        return NoContent();
    }
}