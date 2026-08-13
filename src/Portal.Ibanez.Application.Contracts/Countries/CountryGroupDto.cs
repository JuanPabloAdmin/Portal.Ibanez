using System;

namespace Portal.Ibanez.Countries;

/// <summary>
/// Un país tal y como se muestra en la vista de clientes agrupados.
/// <see cref="CountryId"/> es null en el grupo de clientes que todavía no
/// tienen país asignado.
/// </summary>
public class CountryGroupDto
{
    public Guid? CountryId { get; set; }

    public string Name { get; set; }

    public int CustomerCount { get; set; }
}
