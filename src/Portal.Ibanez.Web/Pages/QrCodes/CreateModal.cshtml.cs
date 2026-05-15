using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.QrCodes;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    [BindProperty]
    public CreateUpdateQrCodeDto QrCode { get; set; }

    private readonly IQrCodeAppService _qrCodeAppService;

    public CreateModalModel(IQrCodeAppService qrCodeAppService)
    {
        _qrCodeAppService = qrCodeAppService;
    }

    public void OnGet()
    {
        QrCode = new CreateUpdateQrCodeDto
        {
            IsActive = true,
            Code = Guid.NewGuid().ToString("N")[..12].ToUpper()
        };

        if (MachineId.HasValue)
        {
            QrCode.MachineId = MachineId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _qrCodeAppService.CreateAsync(QrCode);
        return NoContent();
    }
}