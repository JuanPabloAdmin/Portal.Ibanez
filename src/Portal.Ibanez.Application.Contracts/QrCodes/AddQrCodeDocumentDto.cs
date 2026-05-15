using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.QrCodes;

public class AddQrCodeDocumentDto
{
    [Required]
    public Guid QrCodeId { get; set; }

    [Required]
    [DisplayName("Documento")]
    public Guid MachineDocumentId { get; set; }

    [DisplayName("Orden")]
    public int DisplayOrder { get; set; }
}