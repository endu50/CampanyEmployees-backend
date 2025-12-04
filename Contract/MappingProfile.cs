using AutoMapper;
using CompanyEmployees.Models;
using CompanyEmployees.DTOObject;

namespace CompanyEmployees.Contract
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress,
            opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();

            CreateMap<Department, DeparmentDto>();

            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<EmployeeForCreatingDto, Employee>();
          //  CreateMap <DeparmentForCreatingDto, Department>();
            
        }
    }
}
