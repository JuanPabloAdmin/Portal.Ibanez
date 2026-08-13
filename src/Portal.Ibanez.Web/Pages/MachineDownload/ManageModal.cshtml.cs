using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.MachineDownloadLinks;

namespace Portal.Ibanez.Web.Pages.MachineDownloadLinks;

[Authorize]
public class ManageModalModel : IbanezPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid MachineId { get; set; }

    public MachineDownloadLinkDto? DownloadLink { get; set; }

    private readonly IMachineDownloadLinkAppService
        _machineDownloadLinkAppService;

    public ManageModalModel(
        IMachineDownloadLinkAppService machineDownloadLinkAppService)
    {
        _machineDownloadLinkAppService =
            machineDownloadLinkAppService;
    }

    public async Task OnGetAsync()
    {
        DownloadLink =
            await _machineDownloadLinkAppService.GetAsync(MachineId);

        if (DownloadLink == null)
        {
            await _machineDownloadLinkAppService.GenerateAsync(MachineId);

            DownloadLink =
                await _machineDownloadLinkAppService.GetAsync(MachineId);
        }
    }
}