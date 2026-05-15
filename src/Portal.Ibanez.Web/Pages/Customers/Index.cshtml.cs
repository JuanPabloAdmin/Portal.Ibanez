using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.Customers;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICustomerAppService _customerAppService;

    // Propiedad para almacenar la lista de clientes
    public IReadOnlyList<CustomerDto> Customers { get; set; } = new List<CustomerDto>();

    public IndexModel(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    public async Task OnGetAsync()
    {
        // Recuperamos los datos directamente en la carga de la página
        // Nota: Le ponemos un MaxResultCount alto temporalmente. Si hay miles de clientes, luego implementamos paginación manual.
        var result = await _customerAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 100
        });

        Customers = result.Items;
    }
}