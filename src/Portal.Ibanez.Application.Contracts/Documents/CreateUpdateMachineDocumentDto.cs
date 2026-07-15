using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portal.Ibanez.Documents;

public class CreateUpdateMachineDocumentDto
{
    [Required]
    [DisplayName("Máquina")]
    public Guid MachineId { get; set; }
    [DisplayName("Carpeta")]
    public Guid? DocumentFolderId { get; set; }

    [Required]
    [StringLength(200)]
    [DisplayName("Título")]
    public string Title { get; set; }

    [StringLength(255)]
    [DisplayName("Nombre del archivo")]
    public string FileName { get; set; }

    [StringLength(255)]
    public string StoredFileName { get; set; }

    [StringLength(1000)]
    [DisplayName("Ruta relativa")]
    public string? RelativePath { get; set; }

    [StringLength(100)]
    public string ContentType { get; set; }

    public long FileSize { get; set; }

    public int Version { get; set; }

    [DisplayName("Activo")]
    public bool IsActive { get; set; } = true;
}