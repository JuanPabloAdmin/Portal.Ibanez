using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Machines;
using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.MachineDocuments;
[Authorize]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }
    [BindProperty(SupportsGet = true)]
    public Guid? DocumentFolderId { get; set; }

    public string Title { get; set; } = "Documentos de máquinas";

    private readonly IMachineAppService _machineAppService;
    private readonly IDocumentFolderAppService _documentFolderAppService;

    public IndexModel(
      IMachineAppService machineAppService,
      IDocumentFolderAppService documentFolderAppService)
    {
        _machineAppService = machineAppService;
        _documentFolderAppService = documentFolderAppService;
    }

    public async Task OnGetAsync()
    {
        if (!MachineId.HasValue)
        {
            return;
        }

        var machine = await _machineAppService.GetAsync(MachineId.Value);

        if (DocumentFolderId.HasValue)
        {
            var folder = await _documentFolderAppService.GetAsync(DocumentFolderId.Value);

            Title =
                $"Documentos de la carpeta {folder.Name} - " +
                $"{machine.MachineTypeName} - " +
                $"Pedido {machine.OrderNumber} - " +
                $"Armario {machine.CabinetNumber}";
        }
        else
        {
            Title =
                $"Documentos de {machine.MachineTypeName} - " +
                $"Pedido {machine.OrderNumber} - " +
                $"Armario {machine.CabinetNumber}";
        }
    }
}