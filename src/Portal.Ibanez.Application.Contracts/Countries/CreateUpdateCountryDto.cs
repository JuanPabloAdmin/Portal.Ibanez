using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.Countries;

public class CreateUpdateCountryDto
{
    [Required]
    [StringLength(100)]
    [DisplayName("País")]
    public string Name { get; set; }
}
