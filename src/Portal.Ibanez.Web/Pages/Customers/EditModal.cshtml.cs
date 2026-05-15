using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Customers;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Customers;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateCustomerDto Customer { get; set; }

    private readonly ICustomerAppService _customerAppService;

    public EditModalModel(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    public async Task OnGetAsync()
    {
        var customerDto = await _customerAppService.GetAsync(Id);

        Customer = ObjectMapper.Map<CustomerDto, CreateUpdateCustomerDto>(customerDto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _customerAppService.UpdateAsync(Id, Customer);
        return NoContent();
    }
}