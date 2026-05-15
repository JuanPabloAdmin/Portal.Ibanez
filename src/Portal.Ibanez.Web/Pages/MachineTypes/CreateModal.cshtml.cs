using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.MachineTypes;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineTypes;

[Authorize]
public class CreateModalModel : IbanezPageModel
{
    [BindProperty]
    public CreateUpdateMachineTypeDto MachineType { get; set; }

    private readonly IMachineTypeAppService _machineTypeAppService;

    public CreateModalModel(IMachineTypeAppService machineTypeAppService)
    {
        _machineTypeAppService = machineTypeAppService;
    }

    public void OnGet()
    {
        MachineType = new CreateUpdateMachineTypeDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _machineTypeAppService.CreateAsync(MachineType);
        return NoContent();
    }
}