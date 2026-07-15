using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.DocumentFolders;

namespace Portal.Ibanez.Web.Pages.DocumentFolders;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateDocumentFolderDto Folder { get; set; }

    private readonly IDocumentFolderAppService _documentFolderAppService;

    public EditModalModel(
        IDocumentFolderAppService documentFolderAppService)
    {
        _documentFolderAppService = documentFolderAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _documentFolderAppService.GetAsync(Id);

        Folder = ObjectMapper.Map<
            DocumentFolderDto,
            CreateUpdateDocumentFolderDto
        >(dto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _documentFolderAppService.UpdateAsync(Id, Folder);

        return NoContent();
    }
}