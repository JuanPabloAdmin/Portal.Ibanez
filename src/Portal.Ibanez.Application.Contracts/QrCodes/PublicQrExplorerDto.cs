using System;
using System.Collections.Generic;

namespace Portal.Ibanez.QrCodes;

public class PublicQrExplorerDto
{
    public string QrCode { get; set; } = string.Empty;

    public Guid RootFolderId { get; set; }

    public Guid CurrentFolderId { get; set; }

    public string CurrentFolderName { get; set; } = string.Empty;

    public List<PublicQrFolderDto> Folders { get; set; } = new();

    public List<PublicQrDocumentDto> Documents { get; set; } = new();

    public bool HasFolders => Folders.Count > 0;

    public bool HasDocuments => Documents.Count > 0;
}

public class PublicQrFolderDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class PublicQrDocumentDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}