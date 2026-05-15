using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Machines;
using Portal.Ibanez.Documents;
using Portal.Ibanez.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.IO;

namespace Portal.Ibanez.Web.Pages.Machines;

[Authorize]
public class DetailModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public MachineDto Machine { get; set; } = default!;
    public IReadOnlyList<MachineDocumentDto> Documents { get; set; } = new List<MachineDocumentDto>();
    public IReadOnlyList<QrCodeDto> QrCodes { get; set; } = new List<QrCodeDto>();

    // Diccionario para cargar los documentos enlazados de cada QR
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

    public async Task<IActionResult> OnGetDownloadQrAsync(Guid qrId)
    {
        var qrDto = await _qrCodeAppService.GetAsync(qrId);

        // Generamos la URL que leerá el cliente (ajusta según tu dominio)
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string qrUrl = $"{baseUrl}/q/{qrDto.Code}";

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            // 20 es el tamaño de los módulos del QR
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Retornamos el archivo para que el navegador lo descargue automáticamente
            return File(qrCodeImage, "image/png", $"QR_{qrDto.Name}_{qrDto.Code}.png");
        }
    }

    public async Task OnGetAsync()
    {
        // Carga de datos base de la máquina
        Machine = await _machineAppService.GetAsync(Id);

        // Carga de todos los documentos de la máquina
        var docResult = await _documentAppService.GetListAsync(new GetMachineDocumentListInput { MachineId = Id, MaxResultCount = 1000 });
        Documents = docResult.Items;

        // Carga de todos los códigos QR
        var qrResult = await _qrCodeAppService.GetListAsync(new GetQrCodeListInput { MachineId = Id, MaxResultCount = 1000 });
        QrCodes = qrResult.Items;

        // Carga de relaciones QR-Documento
        foreach (var qr in QrCodes)
        {
            var linkedDocs = await _qrCodeAppService.GetDocumentsAsync(qr.Id);
            QrLinkedDocuments[qr.Id] = linkedDocs;
        }
    }

    // Acción para vincular un documento a un QR
    public async Task<IActionResult> OnPostAddLinkAsync(Guid qrId, Guid docId, Guid machineId)
    {
        await _qrCodeAppService.AddDocumentAsync(new AddQrCodeDocumentDto
        {
            QrCodeId = qrId,
            MachineDocumentId = docId,
            DisplayOrder = 0
        });

        // Redirección con hash para que el JS abra el acordeón correspondiente
        return Redirect($"/Machines/Detail/{machineId}#collapse-{qrId}");
    }

    // Acción para eliminar la vinculación
    public async Task<IActionResult> OnPostRemoveLinkAsync(Guid linkId, Guid machineId, Guid qrId)
    {
        await _qrCodeAppService.RemoveDocumentAsync(linkId);

        return Redirect($"/Machines/Detail/{machineId}#collapse-{qrId}");
    }
}