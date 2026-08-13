using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Portal.Ibanez.Machines;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.MachineDownloadLinks;

[Authorize]
public class MachineDownloadLinkAppService :
    ApplicationService,
    IMachineDownloadLinkAppService
{
    private readonly IRepository<MachineDownloadLink, Guid> _linkRepository;
    private readonly IRepository<Machine, Guid> _machineRepository;
    private readonly IConfiguration _configuration;

    public MachineDownloadLinkAppService(
        IRepository<MachineDownloadLink, Guid> linkRepository,
        IRepository<Machine, Guid> machineRepository,
        IConfiguration configuration)
    {
        _linkRepository = linkRepository;
        _machineRepository = machineRepository;
        _configuration = configuration;
    }

    public async Task<MachineDownloadLinkDto?> GetAsync(Guid machineId)
    {
        var queryable = await _linkRepository.GetQueryableAsync();

        var link = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.MachineId == machineId)
        );

        return link == null
            ? null
            : MapToDto(link);
    }

    public async Task<MachineDownloadLinkDto> GenerateAsync(Guid machineId)
    {
        await ValidateMachineAsync(machineId);

        var existing = await FindByMachineIdAsync(machineId);

        if (existing != null)
        {
            if (!existing.IsActive)
            {
                existing.Activate();

                await _linkRepository.UpdateAsync(
                    existing,
                    autoSave: true
                );
            }

            return MapToDto(existing);
        }

        var link = new MachineDownloadLink(
            GuidGenerator.Create(),
            machineId,
            GenerateCode()
        );

        await _linkRepository.InsertAsync(
            link,
            autoSave: true
        );

        return MapToDto(link);
    }

    public async Task<MachineDownloadLinkDto> RegenerateAsync(Guid machineId)
    {
        await ValidateMachineAsync(machineId);

        var link = await FindByMachineIdAsync(machineId);

        if (link == null)
        {
            return await GenerateAsync(machineId);
        }

        link.Regenerate(GenerateCode());

        await _linkRepository.UpdateAsync(
            link,
            autoSave: true
        );

        return MapToDto(link);
    }

    public async Task DisableAsync(Guid machineId)
    {
        var link = await FindByMachineIdAsync(machineId);

        if (link == null)
        {
            return;
        }

        link.Deactivate();

        await _linkRepository.UpdateAsync(
            link,
            autoSave: true
        );
    }

    private async Task ValidateMachineAsync(Guid machineId)
    {
        var machine = await _machineRepository.GetAsync(machineId);

        if (string.IsNullOrWhiteSpace(machine.OrderNumber))
        {
            throw new BusinessException(
                "MachineDownloadLink:MissingOrderNumber"
            )
            .WithData("MachineId", machineId);
        }
    }

    private async Task<MachineDownloadLink?> FindByMachineIdAsync(
        Guid machineId)
    {
        var queryable = await _linkRepository.GetQueryableAsync();

        return await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.MachineId == machineId)
        );
    }

    private MachineDownloadLinkDto MapToDto(
        MachineDownloadLink link)
    {
        var selfUrl = _configuration["App:SelfUrl"];

        if (string.IsNullOrWhiteSpace(selfUrl))
        {
            throw new BusinessException(
                "MachineDownloadLink:MissingSelfUrl"
            );
        }

        selfUrl = selfUrl.TrimEnd('/');

        return new MachineDownloadLinkDto
        {
            MachineId = link.MachineId,
            Code = link.Code,
            IsActive = link.IsActive,
            DownloadUrl =
                $"{selfUrl}/machine-download/{Uri.EscapeDataString(link.Code)}"
        };
    }

    private static string GenerateCode()
    {
        var bytes = RandomNumberGenerator.GetBytes(24);

        return Convert
            .ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}