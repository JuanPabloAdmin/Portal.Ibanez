using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Ibanez.Machines;
using Portal.Ibanez.QrCodes; // Asegúrate de que esta ruta sea correcta para QrCodeDto e IQrCodeAppService
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portal.Ibanez.Web.Models;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;

namespace Portal.Ibanez.Web.Pages.QrCodes;

[Authorize]
public class IndexModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? MachineId { get; set; }

    public string Title { get; set; } = "Códigos QR";

    // 👉 1. ESTA ES LA LÍNEA CRÍTICA QUE FALTA PARA QUE EL HTML FUNCIONE
    public IReadOnlyList<QrCodeDto> QrCodes { get; set; } = new List<QrCodeDto>();

    private readonly IMachineAppService _machineAppService;

    // 👉 2. DECLARAMOS EL SERVICIO DE CÓDIGOS QR
    private readonly IQrCodeAppService _qrCodeAppService;

    // 👉 3. INYECTAMOS EL SERVICIO EN EL CONSTRUCTOR
    public IndexModel(
        IMachineAppService machineAppService,
        IQrCodeAppService qrCodeAppService)
    {
        _machineAppService = machineAppService;
        _qrCodeAppService = qrCodeAppService;
    }

    public async Task OnGetAsync()
    {
        if (MachineId.HasValue)
        {
            Breadcrumbs = new()
        {
            new BreadcrumbItem
            {
                Text = "Inicio",
                Url = "/"
            },
            new BreadcrumbItem
            {
                Text = "Clientes",
                Url = "/Customers"
            }
        };

            if (!MachineId.HasValue)
            {
                Breadcrumbs.Add(new BreadcrumbItem
                {
                    Text = "Códigos QR",
                    Active = true
                });

                return;
            }

            var machine = await _machineAppService.GetAsync(MachineId.Value);

            var machineText = string.IsNullOrWhiteSpace(machine.OrderNumber)
                ? machine.MachineTypeName
                : $"{machine.MachineTypeName} · Pedido {machine.OrderNumber}";

            Title = $"Códigos QR de {machineText}";

            Breadcrumbs.Add(new BreadcrumbItem
            {
                Text = machine.CustomerCommercialName,
                Url = $"/Machines?customerId={machine.CustomerId}"
            });

            Breadcrumbs.Add(new BreadcrumbItem
            {
                Text = machineText,
                Url = $"/DocumentFolders?machineId={machine.Id}"
            });

            Breadcrumbs.Add(new BreadcrumbItem
            {
                Text = "Códigos QR",
                Active = true
            });
        

        var input = new GetQrCodeListInput
            {
                MachineId = MachineId.Value,
                MaxResultCount = 1000
            };

            var result = await _qrCodeAppService.GetListAsync(input);
            QrCodes = result.Items;
        }
    }
}