using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portal.Ibanez.Documents;

public class MachineDocument : FullAuditedAggregateRoot<Guid>
{
    public Guid MachineId { get; set; }

    public string Title { get; set; }

    public string FileName { get; set; }

    public string StoredFileName { get; set; }

    public string ContentType { get; set; }

    public long FileSize { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }

    protected MachineDocument()
    {

    }

    public MachineDocument(
        Guid id,
        Guid machineId,
        string title,
        string fileName,
        string storedFileName,
        string contentType,
        long fileSize
    ) : base(id)
    {
        MachineId = machineId;
        Title = title;
        FileName = fileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        FileSize = fileSize;
        Version = 1;
        IsActive = true;
    }
}