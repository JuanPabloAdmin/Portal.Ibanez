using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.Countries;

public interface ICountryAppService :
    ICrudAppService<
        CountryDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateCountryDto>
{
}
