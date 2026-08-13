using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Ibanez.Countries;
using Portal.Ibanez.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.Customers;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateCustomerDto Customer { get; set; }

    public List<SelectListItem> Countries { get; set; } = new();

    private readonly ICustomerAppService _customerAppService;
    private readonly ICountryAppService _countryAppService;

    public EditModalModel(
        ICustomerAppService customerAppService,
        ICountryAppService countryAppService)
    {
        _customerAppService = customerAppService;
        _countryAppService = countryAppService;
    }

    public async Task OnGetAsync()
    {
        var customerDto = await _customerAppService.GetAsync(Id);

        Customer = ObjectMapper.Map<CustomerDto, CreateUpdateCustomerDto>(customerDto);

        await FillCountriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await FillCountriesAsync();
            return Page();
        }

        await _customerAppService.UpdateAsync(Id, Customer);
        return NoContent();
    }

    private async Task FillCountriesAsync()
    {
        var countries = await _countryAppService.GetListAsync(
            new PagedAndSortedResultRequestDto { MaxResultCount = 1000 }
        );

        Countries = countries.Items
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToList();
    }
}
