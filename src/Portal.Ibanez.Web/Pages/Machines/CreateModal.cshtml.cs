using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Ibanez.Customers;
using Portal.Ibanez.Machines;
using Portal.Ibanez.MachineTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.Machines;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateMachineDto Machine { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CustomerId { get; set; }

    public List<SelectListItem> Customers { get; set; }

    public List<SelectListItem> MachineTypes { get; set; }

    private readonly IMachineAppService _machineAppService;
    private readonly ICustomerAppService _customerAppService;
    private readonly IMachineTypeAppService _machineTypeAppService;

    public CreateModalModel(
        IMachineAppService machineAppService,
        ICustomerAppService customerAppService,
        IMachineTypeAppService machineTypeAppService)
    {
        _machineAppService = machineAppService;
        _customerAppService = customerAppService;
        _machineTypeAppService = machineTypeAppService;
    }

    public async Task OnGetAsync()
    {
        Machine = new CreateUpdateMachineDto();

        if (CustomerId.HasValue)
        {
            Machine.CustomerId = CustomerId.Value;
        }

        var customers = await _customerAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 1000
        });

        Customers = customers.Items
        .Select(x => new SelectListItem(x.CommercialName, x.Id.ToString()))
        .ToList();

        var machineTypes = await _machineTypeAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 1000
        });

        MachineTypes = machineTypes.Items
     .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
     .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _machineAppService.CreateAsync(Machine);

        return NoContent();
    }
}