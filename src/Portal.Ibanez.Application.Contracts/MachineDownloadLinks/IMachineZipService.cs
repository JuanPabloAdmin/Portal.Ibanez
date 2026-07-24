using System;
using System.Threading.Tasks;

namespace Portal.Ibanez.MachineDownloadLinks;

public interface IMachineZipService
{
    Task<MachineZipResult> CreateAsync(Guid machineId);
}