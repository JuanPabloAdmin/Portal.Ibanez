using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Countries;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Countries;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateCountryDto Country { get; set; }

    private readonly ICountryAppService _countryAppService;

    public CreateModalModel(ICountryAppService countryAppService)
    {
        _countryAppService = countryAppService;
    }

    public void OnGet()
    {
        Country = new CreateUpdateCountryDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _countryAppService.CreateAsync(Country);
        return NoContent();
    }
}
