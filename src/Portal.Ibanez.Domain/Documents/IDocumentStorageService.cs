using System.IO;
using System.Threading.Tasks;

namespace Portal.Ibanez.Documents;

public interface IDocumentStorageService
{
    Task<Stream> OpenReadAsync(MachineDocument document);

    Task<bool> ExistsAsync(MachineDocument document);

    string GetPhysicalPath(MachineDocument document);
}