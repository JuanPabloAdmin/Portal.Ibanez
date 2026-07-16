using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Portal.Ibanez.Documents;
namespace Portal.Ibanez.DocumentFolders;

public class DocumentFolderAppService :
    CrudAppService<
        DocumentFolder,
        DocumentFolderDto,
        Guid,
        GetDocumentFolderListInput,
        CreateUpdateDocumentFolderDto>,
    IDocumentFolderAppService
{


    private readonly IRepository<MachineDocument, Guid> _machineDocumentRepository;



    public DocumentFolderAppService(
      IRepository<DocumentFolder, Guid> repository,
      IRepository<MachineDocument, Guid> machineDocumentRepository)
      : base(repository)
    {
        _machineDocumentRepository = machineDocumentRepository;
    }
    public override async Task<DocumentFolderDto> CreateAsync(
    CreateUpdateDocumentFolderDto input)
    {
        if (input.MachineId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador de la máquina no puede estar vacío.",
                nameof(input.MachineId)
            );
        }

        if (input.ParentFolderId.HasValue)
        {
            var parentFolder = await Repository.GetAsync(
                input.ParentFolderId.Value
            );

            if (parentFolder.MachineId != input.MachineId)
            {
                throw new ArgumentException(
                    "La carpeta padre no pertenece a la misma máquina."
                );
            }
        }

        var folder = new DocumentFolder(
            GuidGenerator.Create(),
            input.MachineId,
            input.Name
        )
        {
            ParentFolderId = input.ParentFolderId,
            Description = input.Description,
            IsActive = input.IsActive
        };

        await Repository.InsertAsync(folder, autoSave: true);

        return ObjectMapper.Map<DocumentFolder, DocumentFolderDto>(folder);
    }
    public async Task<FolderContentStateDto> GetContentStateAsync(Guid folderId)
    {
        var foldersQueryable = await Repository.GetQueryableAsync();
        var documentsQueryable = await _machineDocumentRepository.GetQueryableAsync();

        var subFoldersCount = await AsyncExecuter.CountAsync(
            foldersQueryable.Where(x => x.ParentFolderId == folderId)
        );

        var documentsCount = await AsyncExecuter.CountAsync(
            documentsQueryable.Where(x =>
                x.DocumentFolderId == folderId &&
                x.IsActive
            )
        );

        return new FolderContentStateDto
        {
            SubFoldersCount = subFoldersCount,
            DocumentsCount = documentsCount
        };
    }

    public override async Task<PagedResultDto<DocumentFolderDto>> GetListAsync(
     GetDocumentFolderListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.MachineId.HasValue)
        {
            queryable = queryable.Where(
                x => x.MachineId == input.MachineId.Value
            );
        }

        if (input.OnlyRootFolders)
        {
            queryable = queryable.Where(
                x => x.ParentFolderId == null
            );
        }
        else if (input.ParentFolderId.HasValue)
        {
            queryable = queryable.Where(
                x => x.ParentFolderId == input.ParentFolderId.Value
            );
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var entities = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? "Name")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<DocumentFolderDto>(
            totalCount,
            ObjectMapper.Map<
                List<DocumentFolder>,
                List<DocumentFolderDto>
            >(entities)
        );
    }

    public async Task<FolderExplorerDto> GetExplorerAsync(
    GetFolderExplorerInput input)
    {
        var foldersQueryable = await Repository.GetQueryableAsync();
        var documentsQueryable = await _machineDocumentRepository.GetQueryableAsync();

        var currentFoldersQuery = foldersQueryable
            .Where(x => x.MachineId == input.MachineId);

        if (input.ParentFolderId.HasValue)
        {
            currentFoldersQuery = currentFoldersQuery.Where(
                x => x.ParentFolderId == input.ParentFolderId.Value
            );
        }
        else
        {
            currentFoldersQuery = currentFoldersQuery.Where(
                x => x.ParentFolderId == null
            );
        }

        var folders = await AsyncExecuter.ToListAsync(
            currentFoldersQuery.OrderBy(x => x.Name)
        );

        var result = new List<FolderExplorerItemDto>();

        foreach (var folder in folders)
        {
            var subFoldersCount = await AsyncExecuter.CountAsync(
                foldersQueryable.Where(x =>
                    x.MachineId == input.MachineId &&
                    x.ParentFolderId == folder.Id
                )
            );

            var documentsCount = await AsyncExecuter.CountAsync(
                documentsQueryable.Where(x =>
                    x.MachineId == input.MachineId &&
                    x.DocumentFolderId == folder.Id &&
                    x.IsActive
                )
            );

            result.Add(new FolderExplorerItemDto
            {
                Id = folder.Id,
                MachineId = folder.MachineId,
                ParentFolderId = folder.ParentFolderId,
                Name = folder.Name,
                Description = folder.Description,
                IsActive = folder.IsActive,
                SubFoldersCount = subFoldersCount,
                DocumentsCount = documentsCount
            });
        }
        var state = new FolderContentStateDto();

        if (input.ParentFolderId.HasValue)
        {
            state = await GetContentStateAsync(input.ParentFolderId.Value);
        }

        var currentDocuments = new List<MachineDocumentDto>();

        if (input.ParentFolderId.HasValue)
        {
            var documents = await AsyncExecuter.ToListAsync(
                documentsQueryable
                    .Where(x =>
                        x.MachineId == input.MachineId &&
                        x.DocumentFolderId == input.ParentFolderId.Value &&
                        x.IsActive
                    )
                    .OrderBy(x => x.RelativePath ?? x.FileName)
            );

            currentDocuments = ObjectMapper.Map<
                List<MachineDocument>,
                List<MachineDocumentDto>
            >(documents);
        }

        return new FolderExplorerDto
        {
            State = state,
            Items = result,
            Documents = currentDocuments,
            CanCreateFolder = !state.HasDocuments,
            CanUploadDocuments = !state.HasSubFolders,
            CanUploadFolder = !state.HasSubFolders
        };
    }
}