using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Countries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.Customers;

[Authorize]
public class CustomerAppService :
    CrudAppService<
        Customer,
        CustomerDto,
        Guid,
        GetCustomerListInput,
        CreateUpdateCustomerDto>,
    ICustomerAppService
{
    private readonly IRepository<Country, Guid> _countryRepository;

    public CustomerAppService(
        IRepository<Customer, Guid> repository,
        IRepository<Country, Guid> countryRepository)
        : base(repository)
    {
        _countryRepository = countryRepository;
    }

    public override async Task<PagedResultDto<CustomerDto>> GetListAsync(GetCustomerListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.OnlyWithoutCountry)
        {
            queryable = queryable.Where(x => x.CountryId == null);
        }
        else if (input.CountryId.HasValue)
        {
            queryable = queryable.Where(x => x.CountryId == input.CountryId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? nameof(Customer.CommercialName))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<Customer>, List<CustomerDto>>(items);

        await FillCountryNamesAsync(dtos);

        return new PagedResultDto<CustomerDto>(totalCount, dtos);
    }

    public override async Task<CustomerDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);

        await FillCountryNamesAsync(new List<CustomerDto> { dto });

        return dto;
    }

    public override async Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input)
    {
        await CheckCountryExistsAsync(input.CountryId);

        return await base.CreateAsync(input);
    }

    public override async Task<CustomerDto> UpdateAsync(Guid id, CreateUpdateCustomerDto input)
    {
        await CheckCountryExistsAsync(input.CountryId);

        return await base.UpdateAsync(id, input);
    }

    public async Task<List<CountryGroupDto>> GetCountryGroupsAsync()
    {
        var countryQueryable = await _countryRepository.GetQueryableAsync();
        var customerQueryable = await Repository.GetQueryableAsync();

        var groups = await AsyncExecuter.ToListAsync(
            from country in countryQueryable
            select new CountryGroupDto
            {
                CountryId = country.Id,
                Name = country.Name,
                CustomerCount = customerQueryable.Count(c => c.CountryId == country.Id)
            }
        );

        groups = groups.OrderBy(x => x.Name).ToList();

        var withoutCountryCount = await AsyncExecuter.CountAsync(
            customerQueryable.Where(c => c.CountryId == null)
        );

        if (withoutCountryCount > 0)
        {
            groups.Add(new CountryGroupDto
            {
                CountryId = null,
                Name = "Sin país asignado",
                CustomerCount = withoutCountryCount
            });
        }

        return groups;
    }

    private async Task CheckCountryExistsAsync(Guid? countryId)
    {
        if (!countryId.HasValue)
        {
            return;
        }

        if (await _countryRepository.FindAsync(countryId.Value) == null)
        {
            throw new UserFriendlyException("El país seleccionado no existe.");
        }
    }

    private async Task FillCountryNamesAsync(List<CustomerDto> customers)
    {
        var countryIds = customers
            .Where(x => x.CountryId.HasValue)
            .Select(x => x.CountryId.Value)
            .Distinct()
            .ToList();

        if (!countryIds.Any())
        {
            return;
        }

        var countryQueryable = await _countryRepository.GetQueryableAsync();

        var countries = await AsyncExecuter.ToListAsync(
            countryQueryable.Where(x => countryIds.Contains(x.Id))
        );

        var namesById = countries.ToDictionary(x => x.Id, x => x.Name);

        foreach (var customer in customers.Where(x => x.CountryId.HasValue))
        {
            if (namesById.TryGetValue(customer.CountryId.Value, out var name))
            {
                customer.CountryName = name;
            }
        }
    }
}
