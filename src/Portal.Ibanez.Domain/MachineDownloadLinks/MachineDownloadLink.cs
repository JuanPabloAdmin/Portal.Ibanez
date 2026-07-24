using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.MachineDownloadLinks;

public class MachineDownloadLink : FullAuditedAggregateRoot<Guid>
{
    public Guid MachineId { get; private set; }

    public string Code { get; private set; }

    public bool IsActive { get; private set; }

    protected MachineDownloadLink()
    {
        Code = string.Empty;
    }

    public MachineDownloadLink(
        Guid id,
        Guid machineId,
        string code)
        : base(id)
    {
        MachineId = machineId;
        Code = code;
        IsActive = true;
    }

    public void Regenerate(string newCode)
    {
        Code = newCode;
        IsActive = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}