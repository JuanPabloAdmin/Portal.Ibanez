using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portal.Ibanez.Countries;
using Volo.Abp.Application.Services;

namespace Portal.Ibanez.Customers;

public interface ICustomerAppService :
    ICrudAppService<
        CustomerDto,
        Guid,
        GetCustomerListInput,
        CreateUpdateCustomerDto>
{
    /// <summary>
    /// Todos los países dados de alta, con el número de clientes de cada uno.
    /// Si quedan clientes sin país se añade al final un grupo con CountryId null.
    /// </summary>
    Task<List<CountryGroupDto>> GetCountryGroupsAsync();
}
