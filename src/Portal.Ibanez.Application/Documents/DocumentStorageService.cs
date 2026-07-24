using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Portal.Ibanez.Documents;

public class DocumentStorageService :
    IDocumentStorageService,
    ITransientDependency
{
    private readonly IHostEnvironment _hostEnvironment;

    public DocumentStorageService(
        IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public Task<bool> ExistsAsync(MachineDocument document)
    {
        var physicalPath = GetPhysicalPath(document);

        return Task.FromResult(File.Exists(physicalPath));
    }

    public Task<Stream> OpenReadAsync(MachineDocument document)
    {
        var physicalPath = GetPhysicalPath(document);

        if (!File.Exists(physicalPath))
        {
            throw new BusinessException(
                "DocumentStorage:FileNotFound"
            )
            .WithData("DocumentId", document.Id)
            .WithData("FileName", document.FileName)
            .WithData("StoredFileName", document.StoredFileName)
            .WithData("PhysicalPath", physicalPath);
        }

        Stream stream = new FileStream(
            physicalPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true
        );

        return Task.FromResult(stream);
    }

    public string GetPhysicalPath(MachineDocument document)
    {
        if (document.MachineId == Guid.Empty)
        {
            throw new BusinessException(
                "DocumentStorage:MissingMachineId"
            )
            .WithData("DocumentId", document.Id);
        }

        if (!document.DocumentFolderId.HasValue)
        {
            throw new BusinessException(
                "DocumentStorage:MissingDocumentFolderId"
            )
            .WithData("DocumentId", document.Id)
            .WithData("FileName", document.FileName);
        }

        if (string.IsNullOrWhiteSpace(document.StoredFileName))
        {
            throw new BusinessException(
                "DocumentStorage:MissingStoredFileName"
            )
            .WithData("DocumentId", document.Id)
            .WithData("FileName", document.FileName);
        }

        var webRootPath = Path.Combine(
            _hostEnvironment.ContentRootPath,
            "wwwroot"
        );

        var uploadsRootPath = Path.GetFullPath(
            Path.Combine(
                webRootPath,
                "uploads",
                "machines"
            )
        );

        var physicalPath = Path.GetFullPath(
            Path.Combine(
                uploadsRootPath,
                document.MachineId.ToString(),
                document.DocumentFolderId.Value.ToString(),
                document.StoredFileName
            )
        );

        var allowedRoot =
            uploadsRootPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            ) + Path.DirectorySeparatorChar;

        if (!physicalPath.StartsWith(
                allowedRoot,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                "DocumentStorage:InvalidPhysicalPath"
            )
            .WithData("DocumentId", document.Id)
            .WithData("StoredFileName", document.StoredFileName);
        }

        return physicalPath;
    }
}