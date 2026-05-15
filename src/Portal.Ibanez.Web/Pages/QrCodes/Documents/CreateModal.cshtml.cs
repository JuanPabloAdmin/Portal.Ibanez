using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Ibanez.Documents;
using Portal.Ibanez.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.QrCodes.Documents;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid QrCodeId { get; set; }

    [BindProperty]
    public AddQrCodeDocumentDto Input { get; set; }

    public List<SelectListItem> Documents { get; set; }

    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IMachineDocumentAppService _machineDocumentAppService;

    public CreateModalModel(
        IQrCodeAppService qrCodeAppService,
        IMachineDocumentAppService machineDocumentAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _machineDocumentAppService = machineDocumentAppService;
    }

    public async Task OnGetAsync()
    {
        var qrCode = await _qrCodeAppService.GetAsync(QrCodeId);

        Input = new AddQrCodeDocumentDto
        {
            QrCodeId = QrCodeId,
            DisplayOrder = 0
        };

        var documents = await _machineDocumentAppService.GetListAsync(new GetMachineDocumentListInput
        {
            MachineId = qrCode.MachineId,
            MaxResultCount = 1000
        });

        Documents = documents.Items
            .Select(x => new SelectListItem($"{x.Title} - {x.FileName}", x.Id.ToString()))
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _qrCodeAppService.AddDocumentAsync(Input);
        return NoContent();
    }
}