using System.Collections.Generic;
using Portal.Ibanez.Documents;

namespace Portal.Ibanez.DocumentFolders;

public class FolderExplorerDto
{
    public FolderContentStateDto State { get; set; } = new();

    public List<FolderExplorerItemDto> Items { get; set; } = new();
    public List<MachineDocumentDto> Documents { get; set; } = new();

    public bool CanCreateFolder { get; set; }

    public bool CanUploadDocuments { get; set; }

    public bool CanUploadFolder { get; set; }
}