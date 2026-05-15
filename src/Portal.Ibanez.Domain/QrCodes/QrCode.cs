using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.QrCodes;

public class QrCode : FullAuditedAggregateRoot<Guid>
{
    public Guid MachineId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    protected QrCode()
    {

    }

    public QrCode(
        Guid id,
        Guid machineId,
        string code,
        string name
    ) : base(id)
    {
        MachineId = machineId;
        Code = code;
        Name = name;
        IsActive = true;
    }
}