using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Machines;

public class MachineDto : FullAuditedEntityDto<Guid>
{
    public Guid CustomerId { get; set; }

    public Guid MachineTypeId { get; set; }

    public DateTime? ManufacturingDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string OrderNumber { get; set; }

    public string CabinetNumber { get; set; }

    public string Observations { get; set; }

    public string CustomerCommercialName { get; set; }

    public string MachineTypeName { get; set; }
}