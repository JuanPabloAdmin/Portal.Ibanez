using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Countries;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Countries;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateCountryDto Country { get; set; }

    private readonly ICountryAppService _countryAppService;

    public EditModalModel(ICountryAppService countryAppService)
    {
        _countryAppService = countryAppService;
    }

    public async Task OnGetAsync()
    {
        var countryDto = await _countryAppService.GetAsync(Id);

        Country = ObjectMapper.Map<CountryDto, CreateUpdateCountryDto>(countryDto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _countryAppService.UpdateAsync(Id, Country);
        return NoContent();
    }
}
