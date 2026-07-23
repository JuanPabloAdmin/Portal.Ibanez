using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Machines;
using Portal.Ibanez.Documents; // Namespace de tu AppService
using System;
using System.Collections.Generic;
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

    // Nueva propiedad para almacenar la lista de documentos y enviarla a la vista
    public IReadOnlyList<MachineDocumentDto> Documents { get; set; } = new List<MachineDocumentDto>();

    private readonly IMachineAppService _machineAppService;
    private readonly IDocumentFolderAppService _documentFolderAppService;
    private readonly IMachineDocumentAppService _machineDocumentAppService;

    public IndexModel(
      IMachineAppService machineAppService,
      IDocumentFolderAppService documentFolderAppService,
      IMachineDocumentAppService machineDocumentAppService)
    {
        _machineAppService = machineAppService;
        _documentFolderAppService = documentFolderAppService;
        _machineDocumentAppService = machineDocumentAppService;
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

        // Cargar los documentos desde el servicio
        var input = new GetMachineDocumentListInput
        {
            MachineId = MachineId.Value,
            DocumentFolderId = DocumentFolderId,
            MaxResultCount = 1000 // Aseguramos traer todos para la vista de tarjetas
        };

        var result = await _machineDocumentAppService.GetListAsync(input);
        Documents = result.Items;
    }
}