using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Machines;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    public string Title { get; set; } = "Códigos QR";

    private readonly IMachineAppService _machineAppService;

    public IndexModel(IMachineAppService machineAppService)
    {
        _machineAppService = machineAppService;
    }

    public async Task OnGetAsync()
    {
        if (MachineId.HasValue)
        {
            var machine = await _machineAppService.GetAsync(MachineId.Value);

            Title = $"Códigos QR de {machine.MachineTypeName} - Pedido {machine.OrderNumber} - Armario {machine.CabinetNumber}";
        }
    }
}