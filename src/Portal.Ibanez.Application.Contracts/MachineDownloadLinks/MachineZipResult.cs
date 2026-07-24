using System.IO;

namespace Portal.Ibanez.MachineDownloadLinks;

public class MachineZipResult
{
    public MemoryStream Stream { get; set; } = new();

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/zip";
}