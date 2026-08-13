using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.MachineDownloadLinks;

public interface IMachineDownloadLinkAppService : IApplicationService
{
    Task<MachineDownloadLinkDto?> GetAsync(Guid machineId);

    Task<MachineDownloadLinkDto> GenerateAsync(Guid machineId);

    Task<MachineDownloadLinkDto> RegenerateAsync(Guid machineId);

    Task DisableAsync(Guid machineId);
}