using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.QrCodes;

public class GetQrCodeListInput : PagedAndSortedResultRequestDto
{
    public Guid? MachineId { get; set; }
}