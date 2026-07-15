using Microsoft.AspNetCore.Authorization;
using Portal.Ibanez.Customers;
using Portal.Ibanez.DocumentFolders;
using Portal.Ibanez.Documents;
using Portal.Ibanez.MachineTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Portal.Ibanez.Machines;

[Authorize]
public class MachineAppService :
    CrudAppService<
        Machine,
        MachineDto,
        Guid,
        GetMachineListInput,
        CreateUpdateMachineDto>,
    IMachineAppService
{
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly IRepository<MachineType, Guid> _machineTypeRepository;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    private readonly IRepository<DocumentFolder, Guid> _documentFolderRepository;
    private readonly IRepository<MachineDocument, Guid> _machineDocumentRepository;


    public MachineAppService(
    IRepository<Machine, Guid> repository,
    IRepository<Customer, Guid> customerRepository,
    IRepository<MachineType, Guid> machineTypeRepository,
    IRepository<DocumentFolder, Guid> documentFolderRepository,
    IRepository<MachineDocument, Guid> machineDocumentRepository,
    IAsyncQueryableExecuter asyncExecuter)
    : base(repository)
    {
        _customerRepository = customerRepository;
        _machineTypeRepository = machineTypeRepository;
        _documentFolderRepository = documentFolderRepository;
        _machineDocumentRepository = machineDocumentRepository;
        _asyncExecuter = asyncExecuter;
    }

    public override async Task<PagedResultDto<MachineDto>> GetListAsync(GetMachineListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        var customers = await _customerRepository.GetQueryableAsync();
        var machineTypes = await _machineTypeRepository.GetQueryableAsync();

        var query =
            from machine in queryable
            join customer in customers on machine.CustomerId equals customer.Id
            join machineType in machineTypes on machine.MachineTypeId equals machineType.Id
            select new MachineDto
            {
                Id = machine.Id,

                CustomerId = machine.CustomerId,
                MachineTypeId = machine.MachineTypeId,

                ManufacturingDate = machine.ManufacturingDate,
                DeliveryDate = machine.DeliveryDate,

                OrderNumber = machine.OrderNumber,
                CabinetNumber = machine.CabinetNumber,
                Observations = machine.Observations,

                CustomerCommercialName = customer.CommercialName,
                MachineTypeName = machineType.Name
            };
        if (input.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == input.CustomerId.Value);
        }

        var totalCount = await _asyncExecuter.CountAsync(query);

        var items = await _asyncExecuter.ToListAsync(
     query
         .OrderBy(input.Sorting ?? "OrderNumber")
         .Skip(input.SkipCount)
         .Take(input.MaxResultCount)
 );

        return new PagedResultDto<MachineDto>(
            totalCount,
            items
        );
    }
    public async Task<DuplicateMachineResultDto> DuplicateAsync(
     Guid sourceMachineId,
     DuplicateMachineDto input)
    {
        var sourceMachine = await Repository.GetAsync(sourceMachineId);
        var duplicatedDocuments = new List<DuplicatedDocumentDto>();

        var newMachine = new Machine(
            GuidGenerator.Create(),
            input.CustomerId,
            input.MachineTypeId
        )
        {
            ManufacturingDate = input.ManufacturingDate,
            DeliveryDate = input.DeliveryDate,
            OrderNumber = input.OrderNumber,
            CabinetNumber = input.CabinetNumber,
            Observations = input.Observations
        };

        await Repository.InsertAsync(newMachine, autoSave: true);

        if (input.CopyDocuments)
        {
            var sourceFoldersQueryable =
                await _documentFolderRepository.GetQueryableAsync();

            var sourceFolders = await AsyncExecuter.ToListAsync(
                sourceFoldersQueryable
                    .Where(x => x.MachineId == sourceMachineId)
                    .OrderBy(x => x.Name)
            );

            foreach (var sourceFolder in sourceFolders)
            {
                var newFolder = new DocumentFolder(
                    GuidGenerator.Create(),
                    newMachine.Id,
                    sourceFolder.Name
                )
                {
                    Description = sourceFolder.Description,
                    IsActive = sourceFolder.IsActive
                };

                await _documentFolderRepository.InsertAsync(
                    newFolder,
                    autoSave: true
                );

                var sourceDocumentsQueryable =
                    await _machineDocumentRepository.GetQueryableAsync();

                var sourceDocuments = await AsyncExecuter.ToListAsync(
                    sourceDocumentsQueryable
                        .Where(x =>
                            x.MachineId == sourceMachineId &&
                            x.DocumentFolderId == sourceFolder.Id)
                        .OrderBy(x => x.RelativePath ?? x.FileName)
                );

                foreach (var sourceDocument in sourceDocuments)
                {
                    var newDocument = new MachineDocument(
                        GuidGenerator.Create(),
                        newMachine.Id,
                        sourceDocument.Title,
                        sourceDocument.FileName,
                        sourceDocument.StoredFileName,
                        sourceDocument.ContentType,
                        sourceDocument.FileSize
                    )
                    {
                        DocumentFolderId = newFolder.Id,
                        RelativePath = sourceDocument.RelativePath,
                        Version = sourceDocument.Version,
                        IsActive = sourceDocument.IsActive
                    };

                    await _machineDocumentRepository.InsertAsync(
                        newDocument,
                        autoSave: true
                    );
                    duplicatedDocuments.Add(new DuplicatedDocumentDto
                    {
                        SourceDocumentId = sourceDocument.Id,
                        NewDocumentId = newDocument.Id
                    });
                }
            }
        }

        return new DuplicateMachineResultDto
        {
            Machine = ObjectMapper.Map<Machine, MachineDto>(newMachine),
            Documents = duplicatedDocuments
        };
    }

}