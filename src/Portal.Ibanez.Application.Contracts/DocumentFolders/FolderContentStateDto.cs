namespace Portal.Ibanez.DocumentFolders;

public class FolderContentStateDto
{
    public int SubFoldersCount { get; set; }

    public int DocumentsCount { get; set; }

    public bool HasSubFolders => SubFoldersCount > 0;

    public bool HasDocuments => DocumentsCount > 0;

    public bool IsEmpty => SubFoldersCount == 0 && DocumentsCount == 0;
}