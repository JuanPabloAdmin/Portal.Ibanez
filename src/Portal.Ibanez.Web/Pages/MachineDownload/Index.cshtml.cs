using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portal.Ibanez.MachineDownloadLinks;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Portal.Ibanez.Web.Pages.MachineDownload;

[AllowAnonymous]
public class IndexModel : IbanezPageModel
{
    private readonly IRepository<MachineDownloadLink, Guid> _repository;
    private readonly IMachineZipService _machineZipService;

    public IndexModel(
        IRepository<MachineDownloadLink, Guid> repository,
        IMachineZipService machineZipService)
    {
        _repository = repository;
        _machineZipService = machineZipService;
    }

    public async Task<IActionResult> OnGetAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return NotFound();
        }

        var queryable = await _repository.GetQueryableAsync();

        var link = await queryable.FirstOrDefaultAsync(x =>
            x.Code == code &&
            x.IsActive);

        if (link == null)
        {
            return NotFound();
        }

        var zip = await _machineZipService.CreateAsync(
            link.MachineId);

        zip.Stream.Position = 0;

        return File(
            zip.Stream,
            zip.ContentType,
            zip.FileName);
    }
}