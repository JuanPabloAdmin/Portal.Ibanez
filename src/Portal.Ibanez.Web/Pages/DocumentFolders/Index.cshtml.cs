using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Machines;

namespace Portal.Ibanez.Web.Pages.DocumentFolders;

[Authorize]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    public string Title { get; set; } = "Carpetas documentales";

    private readonly IMachineAppService _machineAppService;

    public IndexModel(IMachineAppService machineAppService)
    {
        _machineAppService = machineAppService;
    }

    public async Task OnGetAsync()
    {
        if (!MachineId.HasValue)
        {
            return;
        }

        var machine = await _machineAppService.GetAsync(MachineId.Value);

        Title =
            $"Carpetas de {machine.MachineTypeName} - " +
            $"Pedido {machine.OrderNumber} - " +
            $"Armario {machine.CabinetNumber}";
    }
}