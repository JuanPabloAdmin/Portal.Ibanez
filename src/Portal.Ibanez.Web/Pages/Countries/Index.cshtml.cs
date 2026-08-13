using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp;
using Portal.Ibanez.Countries;
using Portal.Ibanez.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Countries;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICountryAppService _countryAppService;
    private readonly ICustomerAppService _customerAppService;

    public IReadOnlyList<CountryGroupDto> Countries { get; set; } = new List<CountryGroupDto>();

    public IndexModel(
        ICountryAppService countryAppService,
        ICustomerAppService customerAppService)
    {
        _countryAppService = countryAppService;
        _customerAppService = customerAppService;
    }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        try
        {
            await _countryAppService.DeleteAsync(id);
        }
        catch (UserFriendlyException ex)
        {
            await LoadCountriesAsync();
            ErrorMessage = ex.Message;
            return Page();
        }

        return RedirectToPage();
    }

    public async Task OnGetAsync()
    {
        await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        // Reutilizamos los grupos porque ya traen el número de clientes de cada país;
        // descartamos el grupo "sin país", que aquí no representa un registro real.
        var groups = await _customerAppService.GetCountryGroupsAsync();

        Countries = groups.Where(x => x.CountryId.HasValue).ToList();
    }
}
