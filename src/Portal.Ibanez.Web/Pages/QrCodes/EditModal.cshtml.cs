using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateQrCodeDto QrCode { get; set; }

    private readonly IQrCodeAppService _qrCodeAppService;

    public EditModalModel(IQrCodeAppService qrCodeAppService)
    {
        _qrCodeAppService = qrCodeAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _qrCodeAppService.GetAsync(Id);
        QrCode = ObjectMapper.Map<QrCodeDto, CreateUpdateQrCodeDto>(dto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _qrCodeAppService.UpdateAsync(Id, QrCode);
        return NoContent();
    }
}