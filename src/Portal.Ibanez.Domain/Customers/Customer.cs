using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.Customers;

public class Customer : FullAuditedAggregateRoot<Guid>
{
    public string CommercialName { get; set; }

    public string? FiscalName { get; set; }

    public string? TaxId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? ContactPerson { get; set; }

    /* Opcional: los clientes anteriores a la introducción de los países
     * se agrupan en "Sin país asignado" hasta que se les asigne uno. */
    public Guid? CountryId { get; set; }

    protected Customer()
    {

    }

    public Customer(
        Guid id,
        string commercialName,
        string fiscalName,
        string taxId
    ) : base(id)
    {
        CommercialName = commercialName;
        FiscalName = fiscalName;
        TaxId = taxId;
    }
}