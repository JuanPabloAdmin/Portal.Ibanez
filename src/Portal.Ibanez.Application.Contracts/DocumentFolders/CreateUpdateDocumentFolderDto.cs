using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.DocumentFolders;

public class CreateUpdateDocumentFolderDto
{
    [Required]
    public Guid MachineId { get; set; }
    [DisplayName("Carpeta padre")]
    public Guid? ParentFolderId { get; set; }

    [Required]
    [StringLength(200)]
    [DisplayName("Nombre de la carpeta")]
    public string Name { get; set; }

    [StringLength(1000)]
    [DisplayName("Descripción")]
    public string Description { get; set; }

    [DisplayName("Activa")]
    public bool IsActive { get; set; } = true;
}