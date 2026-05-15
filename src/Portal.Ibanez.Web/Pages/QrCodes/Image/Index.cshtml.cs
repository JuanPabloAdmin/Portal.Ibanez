using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.Machines;
using Portal.Ibanez.MachineTypes;
using Portal.Ibanez.QrCodes;
using QRCoder;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Portal.Ibanez.Web.Pages.QrCodes.Image;

[Authorize]
public class IndexModel : IbanezPageModel
{
    private readonly IQrCodeAppService _qrCodeAppService;
    private readonly IMachineAppService _machineAppService;
    private readonly IMachineTypeAppService _machineTypeAppService;

    public IndexModel(
        IQrCodeAppService qrCodeAppService,
        IMachineAppService machineAppService,
        IMachineTypeAppService machineTypeAppService)
    {
        _qrCodeAppService = qrCodeAppService;
        _machineAppService = machineAppService;
        _machineTypeAppService = machineTypeAppService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var qrCode = await _qrCodeAppService.GetAsync(id);

        var machine = await _machineAppService.GetAsync(qrCode.MachineId);

        var machineType = await _machineTypeAppService.GetAsync(machine.MachineTypeId);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var qrUrl = $"{baseUrl}/q/{qrCode.Code}";

        using var qrGenerator = new QRCodeGenerator();

        using var qrCodeData = qrGenerator.CreateQrCode(
            qrUrl,
            QRCodeGenerator.ECCLevel.Q
        );

        var pngQrCode = new PngByteQRCode(qrCodeData);

        var qrBytes = pngQrCode.GetGraphic(20);

        using var qrBitmap = SKBitmap.Decode(qrBytes);

        int width = 620;
        int height = 720;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));

        var canvas = surface.Canvas;

        canvas.Clear(SKColors.White);

        var titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 28,
            IsAntialias = true,
            FakeBoldText = true
        };

        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 22,
            IsAntialias = true
        };

        string title = "https://www.ibanez.eu";

        canvas.DrawText(title, 140, 50, titlePaint);

        var qrRect = new SKRect(100, 90, 520, 510);

        canvas.DrawBitmap(qrBitmap, qrRect);

        canvas.DrawText(
            $"Tipo de máquina: {machineType.Name}",
            80,
            565,
            textPaint
        );

        canvas.DrawText(
            $"Nº pedido: {machine.OrderNumber}",
            80,
            610,
            textPaint
        );

        canvas.DrawText(
            $"Nº armario: {machine.CabinetNumber}",
            80,
            655,
            textPaint
        );

        using var image = surface.Snapshot();

        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return File(data.ToArray(), "image/png");
    }
}