using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Portal.Ibanez.Machines;

public class CreateUpdateMachineDto
{
    [Required]
    [DisplayName("Cliente")]
    public Guid CustomerId { get; set; }

    [Required]
    [DisplayName("Tipo de máquina")]
    public Guid MachineTypeId { get; set; }

    [DisplayName("Fecha fabricación")]
    public DateTime? ManufacturingDate { get; set; }

    [DisplayName("Fecha entrega")]
    public DateTime? DeliveryDate { get; set; }

    [StringLength(100)]
    [DisplayName("Número de pedido")]
    public string OrderNumber { get; set; }

    [StringLength(100)]
    [DisplayName("Número de armario")]
    public string CabinetNumber { get; set; }

    [StringLength(1000)]
    [DisplayName("Observaciones")]
    public string Observations { get; set; }
}