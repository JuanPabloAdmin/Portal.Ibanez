using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Customers;
using Microsoft.AspNetCore.Authorization;

namespace Portal.Ibanez.Web.Pages.Customers;
[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateCustomerDto Customer { get; set; }

    private readonly ICustomerAppService _customerAppService;

    public CreateModalModel(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    public void OnGet()
    {
        Customer = new CreateUpdateCustomerDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _customerAppService.CreateAsync(Customer);
        return NoContent();
    }
}