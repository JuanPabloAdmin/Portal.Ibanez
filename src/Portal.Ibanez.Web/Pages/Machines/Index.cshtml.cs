using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Customers;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.Machines;


[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? CustomerId { get; set; }

    public string Title { get; set; } = "Máquinas";

    private readonly ICustomerAppService _customerAppService;

    public IndexModel(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    public async Task OnGetAsync()
    {
        if (CustomerId.HasValue)
        {
            var customer = await _customerAppService.GetAsync(CustomerId.Value);
            Title = $"Máquinas de {customer.CommercialName}";
        }
    }
}