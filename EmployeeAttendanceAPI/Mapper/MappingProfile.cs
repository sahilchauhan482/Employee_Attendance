using AutoMapper;
using EmployeeAPI.Data.Entities;
using EmployeeCommon.DTOs;
using EmployeeCommon.Enum;

namespace EmployeeAPI.DtoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto,User>().ReverseMap();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<Employee, EmployeeDto>()
                            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                            .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                            .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.City.StateId))
                            .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.City.State.Name))
                            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.City.State.CountryId))
                            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City.State.Country.Name));
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<State, StateDto>().ReverseMap();
            CreateMap<City, CityDto>().ReverseMap();
            CreateMap<AttendanceDTO, Attendance>();
            CreateMap<Attendance, AttendanceDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : ""))
                .ForMember(dest => dest.EmployeeSalary, opt => opt.MapFrom(src => src.Employee.Salary));


        }
    }
}
