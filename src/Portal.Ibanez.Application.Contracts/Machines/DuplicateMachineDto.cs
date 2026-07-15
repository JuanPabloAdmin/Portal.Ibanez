using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.Machines;

public class DuplicateMachineDto
{
    [Required]
    [DisplayName("Cliente destino")]
    public Guid CustomerId { get; set; }

    [Required]
    [DisplayName("Tipo de máquina")]
    public Guid MachineTypeId { get; set; }

    [DisplayName("Fecha de fabricación")]
    public DateTime? ManufacturingDate { get; set; }

    [DisplayName("Fecha de entrega")]
    public DateTime? DeliveryDate { get; set; }

    [StringLength(100)]
    [DisplayName("Número de pedido")]
    public string? OrderNumber { get; set; }

    [StringLength(100)]
    [DisplayName("Número de armario")]
    public string? CabinetNumber { get; set; }

    [StringLength(1000)]
    [DisplayName("Observaciones")]
    public string? Observations { get; set; }

    [DisplayName("Copiar carpetas y documentos")]
    public bool CopyDocuments { get; set; } = true;
}