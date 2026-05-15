using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Documents;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineDocuments;

[Authorize]
public class EditModalModel : IbanezPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateMachineDocumentDto Document { get; set; }

    private readonly IMachineDocumentAppService _machineDocumentAppService;

    public EditModalModel(IMachineDocumentAppService machineDocumentAppService)
    {
        _machineDocumentAppService = machineDocumentAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _machineDocumentAppService.GetAsync(Id);

        Document = ObjectMapper.Map<MachineDocumentDto, CreateUpdateMachineDocumentDto>(dto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _machineDocumentAppService.UpdateAsync(Id, Document);

        return NoContent();
    }
}