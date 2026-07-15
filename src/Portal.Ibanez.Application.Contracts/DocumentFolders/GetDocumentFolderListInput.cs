using System;
using Volo.Abp.Application.Dtos;

namespace Portal.Ibanez.DocumentFolders;

public class GetDocumentFolderListInput : PagedAndSortedResultRequestDto
{
    public Guid? MachineId { get; set; }
    public Guid? DocumentFolderId { get; set; }
}