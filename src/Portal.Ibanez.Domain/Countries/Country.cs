using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.Countries;

public class Country : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }

    protected Country()
    {

    }

    public Country(Guid id, string name) : base(id)
    {
        Name = name;
    }
}
