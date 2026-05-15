using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.Machines;

public class GetMachineListInput : PagedAndSortedResultRequestDto
{
    public Guid? CustomerId { get; set; }
}