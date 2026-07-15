using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.QrCodes;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    [BindProperty]
    public CreateUpdateQrCodeDto QrCode { get; set; }

    public List<SelectListItem> DocumentFolders { get; set; } = new();


    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IDocumentFolderAppService _documentFolderAppService;

    public CreateModalModel(
     IQrCodeAppService qrCodeAppService,
     IDocumentFolderAppService documentFolderAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _documentFolderAppService = documentFolderAppService;
    }

    public async Task OnGetAsync()
    {
        QrCode = new CreateUpdateQrCodeDto
        {
            IsActive = true,
            Code = Guid.NewGuid().ToString("N")[..12].ToUpper()
        };

        if (MachineId.HasValue)
        {
            QrCode.MachineId = MachineId.Value;

            var folders = await _documentFolderAppService.GetListAsync(
                new GetDocumentFolderListInput
                {
                    MachineId = MachineId.Value,
                    MaxResultCount = 1000
                });

            DocumentFolders = folders.Items
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _qrCodeAppService.CreateAsync(QrCode);
        return NoContent();
    }
}