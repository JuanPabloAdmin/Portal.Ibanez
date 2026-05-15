using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Documents;

public class GetMachineDocumentListInput : PagedAndSortedResultRequestDto
{
    public Guid? MachineId { get; set; }
}