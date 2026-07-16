using System;

namespace Portal.Ibanez.DocumentFolders;

public class GetFolderExplorerInput
{
    public Guid MachineId { get; set; }

    public Guid? ParentFolderId { get; set; }
}