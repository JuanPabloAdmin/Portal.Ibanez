using System;
using Volo.Abp.Domain.Entities;

namespace Portal.Ibanez.QrCodes;

public class QrCodeDocument : Entity<Guid>
{
    public Guid QrCodeId { get; set; }

    public Guid MachineDocumentId { get; set; }

    public int DisplayOrder { get; set; }

    protected QrCodeDocument()
    {

    }

    public QrCodeDocument(
        Guid id,
        Guid qrCodeId,
        Guid machineDocumentId
    ) : base(id)
    {
        QrCodeId = qrCodeId;
        MachineDocumentId = machineDocumentId;
        DisplayOrder = 0;
    }
}