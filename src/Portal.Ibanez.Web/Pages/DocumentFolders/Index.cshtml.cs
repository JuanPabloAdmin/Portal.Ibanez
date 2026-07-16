using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Machines;

namespace Portal.Ibanez.Web.Pages.DocumentFolders;

[Authorize]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? ParentFolderId { get; set; }

    public string Title { get; set; } = "Documentación";

    public string? CurrentFolderName { get; set; }

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

        Title =
            $"Documentación de {machine.MachineTypeName} - " +
            $"Pedido {machine.OrderNumber} - " +
            $"Armario {machine.CabinetNumber}";

        if (ParentFolderId.HasValue)
        {
            var folder = await _documentFolderAppService.GetAsync(
                ParentFolderId.Value
            );

            CurrentFolderName = folder.Name;
        }
    }
}