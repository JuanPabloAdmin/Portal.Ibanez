using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Countries;
using Portal.Ibanez.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Customers;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICustomerAppService _customerAppService;

    /// <summary>País seleccionado. Si es null (y <see cref="SinPais"/> es false)
    /// la página muestra el listado de países en lugar del de clientes.</summary>
    [BindProperty(SupportsGet = true)]
    public Guid? CountryId { get; set; }

    /// <summary>Ver el grupo de clientes que aún no tienen país asignado.</summary>
    [BindProperty(SupportsGet = true)]
    public bool SinPais { get; set; }

    public bool ShowCustomers => CountryId.HasValue || SinPais;

    public string SelectedCountryName { get; set; }

    public IReadOnlyList<CountryGroupDto> CountryGroups { get; set; } = new List<CountryGroupDto>();

    public IReadOnlyList<CustomerDto> Customers { get; set; } = new List<CustomerDto>();

    public IndexModel(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    public async Task OnGetAsync()
    {
        if (!ShowCustomers)
        {
            CountryGroups = await _customerAppService.GetCountryGroupsAsync();
            return;
        }

        var result = await _customerAppService.GetListAsync(new GetCustomerListInput
        {
            MaxResultCount = 100,
            CountryId = CountryId,
            OnlyWithoutCountry = SinPais
        });

        Customers = result.Items;

        SelectedCountryName = SinPais
            ? "Sin país asignado"
            : Customers.FirstOrDefault()?.CountryName ?? await GetCountryNameAsync();
    }

    private async Task<string> GetCountryNameAsync()
    {
        // El país existe pero todavía no tiene clientes: lo buscamos en los grupos.
        var groups = await _customerAppService.GetCountryGroupsAsync();

        return groups.FirstOrDefault(x => x.CountryId == CountryId)?.Name ?? "País";
    }
}
