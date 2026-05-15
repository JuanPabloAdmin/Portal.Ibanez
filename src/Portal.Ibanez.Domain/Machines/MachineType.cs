using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.Machines;

public class MachineType : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }

    protected MachineType()
    {

    }

    public MachineType(Guid id, string name) : base(id)
    {
        Name = name;
    }
}