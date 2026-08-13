using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Hosting;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Documents;
using Portal.Ibanez.Machines;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;


namespace Portal.Ibanez.MachineDownloadLinks;

public class MachineZipService : IMachineZipService
{
    private readonly IRepository<Machine, Guid> _machineRepository;
    private readonly IRepository<DocumentFolder, Guid> _folderRepository;
    private readonly IRepository<MachineDocument, Guid> _documentRepository;
    private readonly IDocumentStorageService _documentStorageService;
    private readonly IAsyncQueryableExecuter _asyncExecuter;

    public MachineZipService(
     IRepository<Machine, Guid> machineRepository,
     IRepository<DocumentFolder, Guid> folderRepository,
     IRepository<MachineDocument, Guid> documentRepository,
     IDocumentStorageService documentStorageService,
     IAsyncQueryableExecuter asyncExecuter)
    {
        _machineRepository = machineRepository;
        _folderRepository = folderRepository;
        _documentRepository = documentRepository;
        _documentStorageService = documentStorageService;
        _asyncExecuter = asyncExecuter;
    }

    public async Task<MachineZipResult> CreateAsync(Guid machineId)
    {
        var machine = await _machineRepository.GetAsync(machineId);

        if (string.IsNullOrWhiteSpace(machine.OrderNumber))
        {
            throw new BusinessException(
                "MachineZip:MissingOrderNumber"
            );
        }

        var foldersQueryable = await _folderRepository.GetQueryableAsync();

        var folders = await _asyncExecuter.ToListAsync(
            foldersQueryable
                .Where(x => x.MachineId == machineId)
                .OrderBy(x => x.CreationTime)
        );

        var documentsQueryable = await _documentRepository.GetQueryableAsync();

        var documents = await _asyncExecuter.ToListAsync(
            documentsQueryable
                .Where(x =>
                    x.MachineId == machineId &&
                    x.IsActive)
                .OrderBy(x => x.FileName)
        );

        var folderPaths = BuildFolderPaths(folders);
        foreach (var document in documents)
        {
            if (!await _documentStorageService.ExistsAsync(document))
            {
                throw new BusinessException(
                    "MachineZip:DocumentNotFound")
                    .WithData("Document", document.FileName)
                    .WithData("RelativePath", document.RelativePath)
                    .WithData("PhysicalPath",
                        _documentStorageService.GetPhysicalPath(document));
            }
        }

        var outputStream = new MemoryStream();

        using (var zipStream = new ZipOutputStream(outputStream))
        {
            zipStream.SetLevel(6);

            // La contraseña del ZIP será siempre el número de pedido.
            zipStream.Password = machine.OrderNumber;

            foreach (var document in documents)
            {
                await AddDocumentAsync(
                    zipStream,
                    document,
                    folderPaths
                );
            }

            zipStream.IsStreamOwner = false;
            zipStream.Finish();
        }

        outputStream.Position = 0;
      
        return new MachineZipResult
        {
            Stream = outputStream,
            FileName = BuildZipFileName(machine),
            ContentType = "application/zip"
        };
    }

    private async Task AddDocumentAsync(
        ZipOutputStream zipStream,
        MachineDocument document,
        IReadOnlyDictionary<Guid, string> folderPaths)
    {



        var folderPath = string.Empty;

        if (document.DocumentFolderId.HasValue)
        {
            folderPaths.TryGetValue(
                document.DocumentFolderId.Value,
                out folderPath
            );
        }

        var documentRelativePath =
            string.IsNullOrWhiteSpace(document.RelativePath)
                ? document.FileName
                : document.RelativePath;

        var zipEntryName = string.IsNullOrWhiteSpace(folderPath)
            ? documentRelativePath
            : $"{folderPath}/{documentRelativePath}";

        zipEntryName = NormalizeZipEntryName(zipEntryName);

        var physicalPath = _documentStorageService.GetPhysicalPath(document);
        var fileInfo = new FileInfo(physicalPath);

        var entry = new ZipEntry(zipEntryName)
        {
            DateTime = fileInfo.LastWriteTime,
            Size = fileInfo.Length,

            // Protección AES-256 de cada documento.
            AESKeySize = 256
        };

        zipStream.PutNextEntry(entry);

        await using (var fileStream =
     await _documentStorageService.OpenReadAsync(document))
        {
            await fileStream.CopyToAsync(zipStream);
        }

        zipStream.CloseEntry();
    }

    private Dictionary<Guid, string> BuildFolderPaths(
        IReadOnlyCollection<DocumentFolder> folders)
    {
        var foldersById = folders.ToDictionary(x => x.Id);

        var result = new Dictionary<Guid, string>();

        foreach (var folder in folders)
        {
            result[folder.Id] = BuildFolderPath(
                folder,
                foldersById
            );
        }

        return result;
    }

    private static string BuildFolderPath(
        DocumentFolder folder,
        IReadOnlyDictionary<Guid, DocumentFolder> foldersById)
    {
        var names = new Stack<string>();
        var visited = new HashSet<Guid>();

        DocumentFolder? current = folder;

        while (current != null)
        {
            if (!visited.Add(current.Id))
            {
                throw new BusinessException(
                    "MachineZip:FolderCycleDetected"
                );
            }

            names.Push(SanitizePathSegment(current.Name));

            if (!current.ParentFolderId.HasValue)
            {
                break;
            }

            if (!foldersById.TryGetValue(
                    current.ParentFolderId.Value,
                    out current))
            {
                break;
            }
        }

        return string.Join("/", names);
    }

  

    private static string BuildZipFileName(Machine machine)
    {
        var orderNumber = SanitizeFileName(machine.OrderNumber);

        return $"Documentacion-Pedido-{orderNumber}.zip";
    }

    private static string NormalizeZipEntryName(string value)
    {
        return value
            .Replace('\\', '/')
            .TrimStart('/');
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();

        var sanitized = new string(
            value
                .Where(x => !invalidCharacters.Contains(x))
                .ToArray()
        );

        return string.IsNullOrWhiteSpace(sanitized)
            ? "Carpeta"
            : sanitized.Trim();
    }

    private static string SanitizeFileName(string value)
    {
        return SanitizePathSegment(value);
    }
}