using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Ibanez.DocumentFolders;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateQrCodeDto QrCode { get; set; }
    public List<SelectListItem> DocumentFolders { get; set; } = new();

    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IDocumentFolderAppService _documentFolderAppService;

    public EditModalModel(
      IQrCodeAppService qrCodeAppService,
      IDocumentFolderAppService documentFolderAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _documentFolderAppService = documentFolderAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _qrCodeAppService.GetAsync(Id);

        QrCode = ObjectMapper.Map<QrCodeDto, CreateUpdateQrCodeDto>(dto);

        var folders = await _documentFolderAppService.GetListAsync(
            new GetDocumentFolderListInput
            {
                MachineId = dto.MachineId,
                MaxResultCount = 1000
            });

        DocumentFolders = folders.Items
            .Select(x => new SelectListItem(
                x.Name,
                x.Id.ToString(),
                x.Id == dto.DocumentFolderId
            ))
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _qrCodeAppService.UpdateAsync(Id, QrCode);
        return NoContent();
    }
}