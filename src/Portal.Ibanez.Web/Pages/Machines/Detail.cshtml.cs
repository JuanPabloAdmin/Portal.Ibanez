using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Machines;
using Portal.Ibanez.Documents;
using Portal.Ibanez.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Web.Pages.Machines;

[Authorize]
public class DetailModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public MachineDto Machine { get; set; }
    public IReadOnlyList<MachineDocumentDto> Documents { get; set; }
    public IReadOnlyList<QrCodeDto> QrCodes { get; set; }

    // Diccionario para almacenar qué documentos están vinculados a cada QR
    // Key: QrCodeId | Value: Lista de documentos vinculados
    public Dictionary<Guid, List<QrCodeDocumentDto>> QrLinkedDocuments { get; set; } = new();

    private readonly IMachineAppService _machineAppService;
    private readonly IMachineDocumentAppService _documentAppService;
    private readonly IQrCodeAppService _qrCodeAppService;

    public DetailModel(
        IMachineAppService machineAppService,
        IMachineDocumentAppService documentAppService,
        IQrCodeAppService qrCodeAppService)
    {
        _machineAppService = machineAppService;
        _documentAppService = documentAppService;
        _qrCodeAppService = qrCodeAppService;
    }

    public async Task OnGetAsync()
    {
        Machine = await _machineAppService.GetAsync(Id);

        var docResult = await _documentAppService.GetListAsync(new GetMachineDocumentListInput { MachineId = Id });
        Documents = docResult.Items;

        var qrResult = await _qrCodeAppService.GetListAsync(new GetQrCodeListInput { MachineId = Id });
        QrCodes = qrResult.Items;

        // Cargamos los enlaces de cada QR
        foreach (var qr in QrCodes)
        {
            var linkedDocs = await _qrCodeAppService.GetDocumentsAsync(qr.Id);
            QrLinkedDocuments[qr.Id] = linkedDocs;
        }
    }

    // Método que se ejecuta al pulsar "Añadir" en la pestaña de enlaces
    public async Task<IActionResult> OnPostAddLinkAsync(Guid qrId, Guid docId, Guid machineId)
    {
        await _qrCodeAppService.AddDocumentAsync(new AddQrCodeDocumentDto
        {
            QrCodeId = qrId,
            MachineDocumentId = docId,
            DisplayOrder = 0 // Ajusta si tienes lógica de ordenamiento
        });

        // Redirigimos a la misma página forzando la apertura de la pestaña 3 (#tab-links)
        return Redirect($"/Machines/Detail/{machineId}#tab-links");
    }

    // Método que se ejecuta al pulsar "Quitar" en la pestaña de enlaces
    public async Task<IActionResult> OnPostRemoveLinkAsync(Guid linkId, Guid machineId)
    {
        await _qrCodeAppService.RemoveDocumentAsync(linkId);

        return Redirect($"/Machines/Detail/{machineId}#tab-links");
    }
}