using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Machines;
using Portal.Ibanez.QrCodes; // Asegúrate de que esta ruta sea correcta para QrCodeDto e IQrCodeAppService
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    public string Title { get; set; } = "Códigos QR";

    // 👉 1. ESTA ES LA LÍNEA CRÍTICA QUE FALTA PARA QUE EL HTML FUNCIONE
    public IReadOnlyList<QrCodeDto> QrCodes { get; set; } = new List<QrCodeDto>();

    private readonly IMachineAppService _machineAppService;

    // 👉 2. DECLARAMOS EL SERVICIO DE CÓDIGOS QR
    private readonly IQrCodeAppService _qrCodeAppService;

    // 👉 3. INYECTAMOS EL SERVICIO EN EL CONSTRUCTOR
    public IndexModel(
        IMachineAppService machineAppService,
        IQrCodeAppService qrCodeAppService)
    {
        _machineAppService = machineAppService;
        _qrCodeAppService = qrCodeAppService;
    }

    public async Task OnGetAsync()
    {
        if (MachineId.HasValue)
        {
            var machine = await _machineAppService.GetAsync(MachineId.Value);
            Title = $"Códigos QR de {machine.MachineTypeName} - Pedido {machine.OrderNumber} - Armario {machine.CabinetNumber}";

            var input = new GetQrCodeListInput
            {
                MachineId = MachineId.Value,
                MaxResultCount = 1000
            };

            var result = await _qrCodeAppService.GetListAsync(input);
            QrCodes = result.Items;
        }
    }
}