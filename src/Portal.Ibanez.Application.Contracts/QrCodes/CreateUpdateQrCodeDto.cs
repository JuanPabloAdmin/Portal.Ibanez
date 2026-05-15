using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.QrCodes;

public class CreateUpdateQrCodeDto
{
    [Required]
    [DisplayName("Máquina")]
    public Guid MachineId { get; set; }

    [Required]
    [StringLength(100)]
    [DisplayName("Código QR")]
    public string Code { get; set; }

    [Required]
    [StringLength(200)]
    [DisplayName("Nombre")]
    public string Name { get; set; }

    [StringLength(1000)]
    [DisplayName("Descripción")]
    public string Description { get; set; }

    [DisplayName("Activo")]
    public bool IsActive { get; set; } = true;
}