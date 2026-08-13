using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Customers;

public class GetCustomerListInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Filtra por país. Combinado con <see cref="OnlyWithoutCountry"/> a false,
    /// null significa "todos los clientes".
    /// </summary>
    public Guid? CountryId { get; set; }

    /// <summary>
    /// Devuelve únicamente los clientes que todavía no tienen país asignado.
    /// </summary>
    public bool OnlyWithoutCountry { get; set; }
}
