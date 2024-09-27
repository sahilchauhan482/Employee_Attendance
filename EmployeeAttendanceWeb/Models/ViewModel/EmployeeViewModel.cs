using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EmployeeWeb.Models.ViewModel
{
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {
            employees = new EmployeeDto();
            CountryList = new List<CountryDto>();
            StateList = new List<StateDto>();
            CityList = new List<CityDto>();
            employeeBDto = new List<EmployeeBDto>();
            employeesAniversariesDTOs = new List<EmployeesAniversariesDTO>();
            EmployeeAttendance = new List<AttendanceDTO>();


        }

        public List<AttendanceDTO> EmployeeAttendance { get; set; }
        public List<EmployeeBDto> employeeBDto { get; set; }
        public List<EmployeesAniversariesDTO> employeesAniversariesDTOs { get; set; }
        public EmployeeDto employees { get; set; }
        public IEnumerable<CountryDto> CountryList { get; set; }

        public IEnumerable<StateDto> StateList { get; set; }

        public IEnumerable<CityDto> CityList { get; set; }

    }
}
