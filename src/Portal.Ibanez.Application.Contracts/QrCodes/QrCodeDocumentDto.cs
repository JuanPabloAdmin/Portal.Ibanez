using System;

namespace Portal.Ibanez.QrCodes;

public class QrCodeDocumentDto
{
    public Guid Id { get; set; }

    public Guid QrCodeId { get; set; }

    public Guid MachineDocumentId { get; set; }

    public string DocumentTitle { get; set; }

    public string FileName { get; set; }

    public int DisplayOrder { get; set; }
}