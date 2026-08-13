using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Customers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.Countries;

[Authorize]
public class CountryAppService :
    CrudAppService<
        Country,
        CountryDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateCountryDto>,
    ICountryAppService
{
    private readonly IRepository<Customer, Guid> _customerRepository;

    public CountryAppService(
        IRepository<Country, Guid> repository,
        IRepository<Customer, Guid> customerRepository)
        : base(repository)
    {
        _customerRepository = customerRepository;
    }

    public override async Task<PagedResultDto<CountryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        input.Sorting ??= nameof(Country.Name);

        return await base.GetListAsync(input);
    }

    public override async Task<CountryDto> CreateAsync(CreateUpdateCountryDto input)
    {
        await CheckNameIsFreeAsync(input.Name);

        return await base.CreateAsync(input);
    }

    public override async Task<CountryDto> UpdateAsync(Guid id, CreateUpdateCountryDto input)
    {
        await CheckNameIsFreeAsync(input.Name, id);

        return await base.UpdateAsync(id, input);
    }

    public override async Task DeleteAsync(Guid id)
    {
        // La FK es Restrict: sin este control el borrado reventaría con un error de base de datos.
        if (await _customerRepository.AnyAsync(x => x.CountryId == id))
        {
            throw new UserFriendlyException(
                "No se puede eliminar el país porque tiene clientes asignados. " +
                "Reasigna primero esos clientes a otro país."
            );
        }

        await base.DeleteAsync(id);
    }

    private async Task CheckNameIsFreeAsync(string name, Guid? excludedId = null)
    {
        var queryable = await Repository.GetQueryableAsync();

        var exists = await AsyncExecuter.AnyAsync(
            queryable.Where(x =>
                x.Name.ToLower() == name.ToLower() &&
                (excludedId == null || x.Id != excludedId.Value))
        );

        if (exists)
        {
            throw new UserFriendlyException($"Ya existe un país con el nombre '{name}'.");
        }
    }
}
