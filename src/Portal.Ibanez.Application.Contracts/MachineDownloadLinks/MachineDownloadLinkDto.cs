using System;

namespace Portal.Ibanez.MachineDownloadLinks;

public class MachineDownloadLinkDto
{
    public Guid MachineId { get; set; }

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string DownloadUrl { get; set; } = string.Empty;
}