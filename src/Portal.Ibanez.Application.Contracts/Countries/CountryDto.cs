using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Countries;

public class CountryDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
}
