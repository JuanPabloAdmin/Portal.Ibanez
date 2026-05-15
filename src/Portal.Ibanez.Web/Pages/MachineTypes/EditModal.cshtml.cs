using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.MachineTypes;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineTypes;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateMachineTypeDto MachineType { get; set; }

    private readonly IMachineTypeAppService _machineTypeAppService;

    public EditModalModel(IMachineTypeAppService machineTypeAppService)
    {
        _machineTypeAppService = machineTypeAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _machineTypeAppService.GetAsync(Id);

        MachineType = ObjectMapper.Map<MachineTypeDto, CreateUpdateMachineTypeDto>(dto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _machineTypeAppService.UpdateAsync(Id, MachineType);
        return NoContent();
    }
}