using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Customers;
using System;
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
        PagedAndSortedResultRequestDto,
        CreateUpdateCustomerDto>,
    ICustomerAppService
{
    public CustomerAppService(IRepository<Customer, Guid> repository)
        : base(repository)
    {
    }
}