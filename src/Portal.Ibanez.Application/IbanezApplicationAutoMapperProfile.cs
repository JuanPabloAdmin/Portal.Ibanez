using AutoMapper;
using Portal.Ibanez.Customers;
namespace Portal.Ibanez;
using Portal.Ibanez.MachineTypes;
using Portal.Ibanez.Machines;
using Portal.Ibanez.Documents;
using Portal.Ibanez.QrCodes;


public class IbanezApplicationAutoMapperProfile : Profile
{
    public IbanezApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateUpdateCustomerDto, Customer>();
        CreateMap<CustomerDto, CreateUpdateCustomerDto>();
        CreateMap<MachineType, MachineTypeDto>();
        CreateMap<CreateUpdateMachineTypeDto, MachineType>();
        CreateMap<MachineTypeDto, CreateUpdateMachineTypeDto>();
        CreateMap<Machine, MachineDto>();
        CreateMap<CreateUpdateMachineDto, Machine>();
        CreateMap<MachineDto, CreateUpdateMachineDto>();
        CreateMap<MachineDocument, MachineDocumentDto>();
        CreateMap<CreateUpdateMachineDocumentDto, MachineDocument>();
        CreateMap<MachineDocumentDto, CreateUpdateMachineDocumentDto>();
        CreateMap<QrCode, QrCodeDto>();
        CreateMap<CreateUpdateQrCodeDto, QrCode>();
        CreateMap<QrCodeDto, CreateUpdateQrCodeDto>();
    }
}
