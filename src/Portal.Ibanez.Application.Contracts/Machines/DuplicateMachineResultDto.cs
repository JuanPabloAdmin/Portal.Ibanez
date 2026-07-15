using System;
using System.Collections.Generic;

namespace Portal.Ibanez.Machines;

public class DuplicateMachineResultDto
{
    public MachineDto Machine { get; set; }

    public List<DuplicatedDocumentDto> Documents { get; set; } = new();
}

public class DuplicatedDocumentDto
{
    public Guid SourceDocumentId { get; set; }

    public Guid NewDocumentId { get; set; }
}