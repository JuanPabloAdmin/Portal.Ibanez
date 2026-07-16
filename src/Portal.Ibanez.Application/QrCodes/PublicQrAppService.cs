using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Portal.Ibanez.QrCodes;

public class PublicQrAppService :
    ApplicationService,
    IPublicQrAppService
{
    private readonly IRepository<QrCode, Guid> _qrRepository;
    private readonly IRepository<DocumentFolder, Guid> _folderRepository;
    private readonly IRepository<MachineDocument, Guid> _documentRepository;

    public PublicQrAppService(
        IRepository<QrCode, Guid> qrRepository,
        IRepository<DocumentFolder, Guid> folderRepository,
        IRepository<MachineDocument, Guid> documentRepository)
    {
        _qrRepository = qrRepository;
        _folderRepository = folderRepository;
        _documentRepository = documentRepository;
    }
    private async Task<bool> FolderBelongsToRootAsync(
    Guid rootFolderId,
    Guid folderId)
    {
        var currentFolderId = folderId;
        var visitedFolderIds = new HashSet<Guid>();

        while (true)
        {
            if (!visitedFolderIds.Add(currentFolderId))
            {
                return false;
            }

            if (currentFolderId == rootFolderId)
            {
                return true;
            }

            var currentFolder = await _folderRepository.FindAsync(currentFolderId);

            if (currentFolder == null)
            {
                return false;
            }

            if (!currentFolder.ParentFolderId.HasValue)
            {
                return false;
            }

            currentFolderId = currentFolder.ParentFolderId.Value;
        }
    }
    public async Task<PublicQrExplorerDto?> GetExplorerAsync(
    string code,
    Guid? folderId)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var qrQueryable = await _qrRepository.GetQueryableAsync();

        var qr = await AsyncExecuter.FirstOrDefaultAsync(
            qrQueryable.Where(x =>
                x.Code == code &&
                x.IsActive)
        );

        if (qr == null || !qr.DocumentFolderId.HasValue)
        {
            return null;
        }

        var rootFolderId = qr.DocumentFolderId.Value;
        var currentFolderId = folderId ?? rootFolderId;

        var belongsToRoot = await FolderBelongsToRootAsync(
            rootFolderId,
            currentFolderId
        );

        if (!belongsToRoot)
        {
            return null;
        }

        var currentFolder = await _folderRepository.FindAsync(currentFolderId);

        if (currentFolder == null || !currentFolder.IsActive)
        {
            return null;
        }

        var foldersQueryable = await _folderRepository.GetQueryableAsync();

        var childFolders = await AsyncExecuter.ToListAsync(
            foldersQueryable
                .Where(x =>
                    x.ParentFolderId == currentFolderId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
        );

        var documentsQueryable = await _documentRepository.GetQueryableAsync();

        var documents = await AsyncExecuter.ToListAsync(
            documentsQueryable
                .Where(x =>
                    x.DocumentFolderId == currentFolderId &&
                    x.IsActive)
                .OrderBy(x => x.RelativePath ?? x.FileName)
        );

        return new PublicQrExplorerDto
        {
            QrCode = code,
            RootFolderId = rootFolderId,
            CurrentFolderId = currentFolderId,
            CurrentFolderName = currentFolder.Name,

            Folders = childFolders
                .Select(x => new PublicQrFolderDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToList(),

            Documents = documents
                .Select(x => new PublicQrDocumentDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    FileName = x.FileName
                })
                .ToList()
        };
    }

    public async Task<bool> CanDownloadAsync(
    string code,
    Guid documentId)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var qrQueryable = await _qrRepository.GetQueryableAsync();

        var qr = await AsyncExecuter.FirstOrDefaultAsync(
            qrQueryable.Where(x =>
                x.Code == code &&
                x.IsActive)
        );

        if (qr == null || !qr.DocumentFolderId.HasValue)
        {
            return false;
        }

        var document = await _documentRepository.FindAsync(documentId);

        if (document == null ||
            !document.IsActive ||
            !document.DocumentFolderId.HasValue)
        {
            return false;
        }

        return await FolderBelongsToRootAsync(
            qr.DocumentFolderId.Value,
            document.DocumentFolderId.Value
        );
    }
}