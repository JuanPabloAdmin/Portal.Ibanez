using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Ibanez.Customers;
using Portal.Ibanez.Machines;
using Volo.Abp.Application.Dtos;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Portal.Ibanez.Documents;
using Volo.Abp;

namespace Portal.Ibanez.Web.Pages.Machines;

[Authorize]
public class DuplicateModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid SourceMachineId { get; set; }

    [BindProperty]
    public DuplicateMachineDto Input { get; set; } = new();

    public List<SelectListItem> Customers { get; set; } = new();

    public string MachineSummary { get; set; } = string.Empty;

    private readonly IMachineAppService _machineAppService;
    private readonly ICustomerAppService _customerAppService;

    private readonly IMachineDocumentAppService _machineDocumentAppService;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public DuplicateModalModel(
     IMachineAppService machineAppService,
     ICustomerAppService customerAppService,
     IMachineDocumentAppService machineDocumentAppService,
     IWebHostEnvironment webHostEnvironment)
    {
        _machineAppService = machineAppService;
        _customerAppService = customerAppService;
        _machineDocumentAppService = machineDocumentAppService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task OnGetAsync()
    {
        var machine = await _machineAppService.GetAsync(SourceMachineId);

        Input = new DuplicateMachineDto
        {
            CustomerId = machine.CustomerId,
            MachineTypeId = machine.MachineTypeId,
            ManufacturingDate = machine.ManufacturingDate,
            DeliveryDate = machine.DeliveryDate,
            OrderNumber = machine.OrderNumber,
            CabinetNumber = machine.CabinetNumber,
            Observations = machine.Observations,
            CopyDocuments = true
        };

        MachineSummary =
            $"{machine.MachineTypeName} - " +
            $"Pedido {machine.OrderNumber} - " +
            $"Armario {machine.CabinetNumber}";

        var customers = await _customerAppService.GetListAsync(
            new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 1000,
                Sorting = "CommercialName"
            });

        Customers = customers.Items
            .Select(x => new SelectListItem(
                x.CommercialName,
                x.Id.ToString(),
                x.Id == machine.CustomerId
            ))
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _machineAppService.DuplicateAsync(
            SourceMachineId,
            Input
        );

        if (!Input.CopyDocuments || result.Documents.Count == 0)
        {
            return NoContent();
        }

        var copiedPhysicalFiles = new List<string>();

        try
        {
            foreach (var duplicatedDocument in result.Documents)
            {
                var sourceDocument =
                    await _machineDocumentAppService.GetAsync(
                        duplicatedDocument.SourceDocumentId
                    );

                var newDocument =
                    await _machineDocumentAppService.GetAsync(
                        duplicatedDocument.NewDocumentId
                    );

                var sourceFullPath = BuildDocumentFullPath(sourceDocument);

                if (!System.IO.File.Exists(sourceFullPath))
                {
                    throw new UserFriendlyException(
                        $"No se encontró el archivo físico '{sourceDocument.FileName}'."
                    );
                }

                if (!newDocument.DocumentFolderId.HasValue)
                {
                    throw new UserFriendlyException(
                        $"El documento duplicado '{newDocument.FileName}' no tiene carpeta documental."
                    );
                }

                var destinationDirectory = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads",
                    "machines",
                    result.Machine.Id.ToString(),
                    newDocument.DocumentFolderId.Value.ToString()
                );

                Directory.CreateDirectory(destinationDirectory);

                var extension = Path.GetExtension(sourceDocument.FileName);

                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".pdf";
                }

                var newStoredFileName =
                    $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

                var destinationFullPath = Path.Combine(
                    destinationDirectory,
                    newStoredFileName
                );

                System.IO.File.Copy(
                    sourceFullPath,
                    destinationFullPath,
                    overwrite: false
                );

                copiedPhysicalFiles.Add(destinationFullPath);

                var updateDocument = new CreateUpdateMachineDocumentDto
                {
                    MachineId = newDocument.MachineId,
                    DocumentFolderId = newDocument.DocumentFolderId,
                    Title = newDocument.Title,
                    FileName = newDocument.FileName,
                    StoredFileName = newStoredFileName,
                    RelativePath = newDocument.RelativePath,
                    ContentType = newDocument.ContentType,
                    FileSize = newDocument.FileSize,
                    Version = newDocument.Version,
                    IsActive = newDocument.IsActive
                };

                await _machineDocumentAppService.UpdateAsync(
                    newDocument.Id,
                    updateDocument
                );
            }
        }
        catch
        {
            foreach (var copiedFile in copiedPhysicalFiles)
            {
                if (System.IO.File.Exists(copiedFile))
                {
                    System.IO.File.Delete(copiedFile);
                }
            }

            throw;
        }

        return NoContent();
    }
    private string BuildDocumentFullPath(MachineDocumentDto document)
    {
        if (document.DocumentFolderId.HasValue)
        {
            return Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                "machines",
                document.MachineId.ToString(),
                document.DocumentFolderId.Value.ToString(),
                document.StoredFileName
            );
        }

        return Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "machines",
            document.MachineId.ToString(),
            document.StoredFileName
        );
    }
}