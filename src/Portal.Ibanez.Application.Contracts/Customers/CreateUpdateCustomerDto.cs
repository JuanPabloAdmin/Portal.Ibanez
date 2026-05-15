using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.Customers;

public class CreateUpdateCustomerDto
{
    [Required]
    [StringLength(200)]
    [DisplayName("Nombre comercial")]
    public string CommercialName { get; set; }

    [StringLength(200)]
    [DisplayName("Nombre fiscal")]
    public string FiscalName { get; set; }

    [StringLength(50)]
    [DisplayName("CIF")]
    public string TaxId { get; set; }

    [StringLength(500)]
    [DisplayName("Dirección")]
    public string Address { get; set; }

    [StringLength(50)]
    [DisplayName("Teléfono")]
    public string Phone { get; set; }

    [StringLength(200)]
    [DisplayName("Correo electrónico")]
    public string Email { get; set; }

    [StringLength(200)]
    [DisplayName("Persona de contacto")]
    public string ContactPerson { get; set; }
}