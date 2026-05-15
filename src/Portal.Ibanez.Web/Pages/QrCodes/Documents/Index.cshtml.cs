using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Machines;
using Portal.Ibanez.QrCodes;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes.Documents;

[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid QrCodeId { get; set; }

    public string Title { get; set; } = "Documentos asociados al QR";

    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IMachineAppService _machineAppService;

    public IndexModel(
        IQrCodeAppService qrCodeAppService,
        IMachineAppService machineAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _machineAppService = machineAppService;
    }

    public async Task OnGetAsync()
    {
        var qrCode = await _qrCodeAppService.GetAsync(QrCodeId);
        var machine = await _machineAppService.GetAsync(qrCode.MachineId);

        Title = $"Documentos del QR {qrCode.Name} - {machine.MachineTypeName} - Pedido {machine.OrderNumber} - Armario {machine.CabinetNumber}";
    }
}