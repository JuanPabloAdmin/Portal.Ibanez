using System;
using Portal.Ibanez.Customers;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.Machines;

public class Machine : FullAuditedAggregateRoot<Guid>
{
    public Guid CustomerId { get; set; }

    public Guid MachineTypeId { get; set; }

    public DateTime? ManufacturingDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string OrderNumber { get; set; }

    public string CabinetNumber { get; set; }

    public string Observations { get; set; }

    protected Machine()
    {

    }

    public Machine(
        Guid id,
        Guid customerId,
        Guid machineTypeId
    ) : base(id)
    {
        CustomerId = customerId;
        MachineTypeId = machineTypeId;
    }
}