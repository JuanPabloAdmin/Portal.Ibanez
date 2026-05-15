using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Portal.Ibanez.MachineTypes;

public class CreateUpdateMachineTypeDto
{
    [Required]
    [StringLength(200)]
    [DisplayName("Nombre")]
    public string Name { get; set; }
}