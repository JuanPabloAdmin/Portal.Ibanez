using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Documents;

public class GetMachineDocumentListInput : PagedAndSortedResultRequestDto
{
    public Guid? MachineId { get; set; }
    public Guid? DocumentFolderId { get; set; }
}