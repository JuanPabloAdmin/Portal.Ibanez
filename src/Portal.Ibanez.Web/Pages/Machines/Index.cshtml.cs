using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Customers;
using Portal.Ibanez.Machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Machines;

[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? CustomerId { get; set; }

    public string Title { get; set; } = "Máquinas";

    // Propiedad para almacenar la lista de máquinas que leerá la vista HTML
    public IReadOnlyList<MachineDto> Machines { get; set; } = new List<MachineDto>();

    private readonly ICustomerAppService _customerAppService;
    private readonly IMachineAppService _machineAppService;

    public IndexModel(
        ICustomerAppService customerAppService,
        IMachineAppService machineAppService)
    {
        _customerAppService = customerAppService;
        _machineAppService = machineAppService;
    }

    public async Task OnGetAsync()
    {
        // 1. Si venimos desde el directorio de un cliente, actualizamos el título
        if (CustomerId.HasValue)
        {
            var customer = await _customerAppService.GetAsync(CustomerId.Value);
            Title = $"Máquinas de {customer.CommercialName}";
        }

        // 2. Recuperamos las máquinas usando tu DTO de entrada personalizado
        var result = await _machineAppService.GetListAsync(new GetMachineListInput
        {
            MaxResultCount = 1000 // Traemos un máximo alto (en el futuro se puede paginar)
        });

        // 3. Filtramos la lista si estamos viendo solo las de un cliente específico
        if (CustomerId.HasValue)
        {
            Machines = result.Items.Where(m => m.CustomerId == CustomerId.Value).ToList();
        }
        else
        {
            // Si no hay CustomerId en la URL, mostramos todo el parque de máquinas
            Machines = result.Items;
        }
    }
}