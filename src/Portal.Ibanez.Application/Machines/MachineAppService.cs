using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Customers;
using Portal.Ibanez.MachineTypes;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Portal.Ibanez.Machines;

[Authorize]
public class MachineAppService :
    CrudAppService<
        Machine,
        MachineDto,
        Guid,
        GetMachineListInput,
        CreateUpdateMachineDto>,
    IMachineAppService
{
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly IRepository<MachineType, Guid> _machineTypeRepository;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    public MachineAppService(
     IRepository<Machine, Guid> repository,
     IRepository<Customer, Guid> customerRepository,
     IRepository<MachineType, Guid> machineTypeRepository,
     IAsyncQueryableExecuter asyncExecuter)
     : base(repository)
    {
        _customerRepository = customerRepository;
        _machineTypeRepository = machineTypeRepository;
        _asyncExecuter = asyncExecuter;
    }

    public override async Task<PagedResultDto<MachineDto>> GetListAsync(GetMachineListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        var customers = await _customerRepository.GetQueryableAsync();
        var machineTypes = await _machineTypeRepository.GetQueryableAsync();

        var query =
            from machine in queryable
            join customer in customers on machine.CustomerId equals customer.Id
            join machineType in machineTypes on machine.MachineTypeId equals machineType.Id
            select new MachineDto
            {
                Id = machine.Id,

                CustomerId = machine.CustomerId,
                MachineTypeId = machine.MachineTypeId,

                ManufacturingDate = machine.ManufacturingDate,
                DeliveryDate = machine.DeliveryDate,

                OrderNumber = machine.OrderNumber,
                CabinetNumber = machine.CabinetNumber,
                Observations = machine.Observations,

                CustomerCommercialName = customer.CommercialName,
                MachineTypeName = machineType.Name
            };
        if (input.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == input.CustomerId.Value);
        }

        var totalCount = await _asyncExecuter.CountAsync(query);

        var items = await _asyncExecuter.ToListAsync(
     query
         .OrderBy(input.Sorting ?? "OrderNumber")
         .Skip(input.SkipCount)
         .Take(input.MaxResultCount)
 );

        return new PagedResultDto<MachineDto>(
            totalCount,
            items
        );
    }
}