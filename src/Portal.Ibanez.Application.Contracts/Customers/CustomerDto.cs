using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Customers;

public class CustomerDto : FullAuditedEntityDto<Guid>
{
    public string CommercialName { get; set; }

    public string FiscalName { get; set; }

    public string TaxId { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string ContactPerson { get; set; }
}