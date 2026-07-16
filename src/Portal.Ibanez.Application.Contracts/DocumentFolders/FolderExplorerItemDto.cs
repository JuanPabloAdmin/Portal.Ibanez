using System;

namespace Portal.Ibanez.DocumentFolders;

public class FolderExplorerItemDto
{
    public Guid Id { get; set; }

    public Guid MachineId { get; set; }

    public Guid? ParentFolderId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int SubFoldersCount { get; set; }

    public int DocumentsCount { get; set; }

    public bool HasSubFolders => SubFoldersCount > 0;

    public bool HasDocuments => DocumentsCount > 0;
}