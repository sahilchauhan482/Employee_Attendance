using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Data;

using EmployeeAPI.Repository.IRepository;
using EmployeeAPI.Data.Entities;
using EmployeeCommon.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Web.Helpers;

namespace EmployeeAPI.Controllers
{  
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class EmployeeController : ControllerBase
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
       
        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var empdetail = await _unitOfWork.Employee.FirstOrDefault(x => x.Id == id, "City,City.State,City.State.Country");
            if (empdetail != null)
            {
                var EmployeeDto = _mapper.Map<EmployeeDto>(empdetail);
                return Ok(EmployeeDto);
            }
            return BadRequest();
        }

        ////[HttpGet("{email}")]
        ////public async Task<IActionResult> Get(string email)
        ////{
        ////    var empdetail = await _unitOfWork.Employee.FirstOrDefault(x => x.Email == email, "City,City.State,City.State.Country");
        ////    if (empdetail != null)
        ////    {
        ////        var EmployeeDto = _mapper.Map<EmployeeDto>(empdetail);
        ////        return Ok(EmployeeDto);
        ////    }
        ////    return BadRequest();
        ////}

        [HttpGet]
       
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> Get()
        {
            var result =await _unitOfWork.Employee.GetAll(x => x.IsActive, IncludeProperties: "City,City.State,City.State.Country");
            var allEmoloyees = _mapper.Map<IEnumerable<EmployeeDto>>(result);
            return Ok(allEmoloyees);
        }

        [HttpPost]
        public IActionResult Post([FromBody] EmployeeDto employeeDto)
        {

            if (employeeDto == null) return NotFound();

            var existingEmail = _unitOfWork.Employee.FirstOrDefault(u => u.Email == employeeDto.Email && u.Id != employeeDto.Id);
            if (existingEmail == null) return NotFound("EmailID already Exist ");
            var existingPhone = _unitOfWork.Employee.FirstOrDefault(u => u.MobileNumber == employeeDto.MobileNumber && u.Id != employeeDto.Id);
            if (existingPhone == null) return NotFound("Mobile Number already Exist ");
            var existingPan = _unitOfWork.Employee.FirstOrDefault(u => u.PanNumber == employeeDto.PanNumber && u.Id != employeeDto.Id);
            if (existingPan == null) return NotFound("PAN Number already Exist ");
            var employee = _mapper.Map<Employee>(employeeDto);

            employee.CreatedDate = DateTime.Now;
           

            _unitOfWork.Employee.Add(employee);

            if (employee.IsActive)
            {
                employee.RelievingDate = null;
            }

            _unitOfWork.save();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _unitOfWork.Employee.GetById(id);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }
            if (employee.IsActive)
            {
                employee.IsActive = false;
                employee.RelievingDate = DateTime.Now;
               
            }
            _unitOfWork.Employee.Update(employee);
            _unitOfWork.save();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeDto employeeDto)
        {
            var existingEmail = _unitOfWork.Employee.FirstOrDefault(u => u.Email == employeeDto.Email && u.Id != employeeDto.Id);
            if (existingEmail == null) return NotFound("EmailID already Exist ");
            var existingPhone = _unitOfWork.Employee.FirstOrDefault(u => u.MobileNumber == employeeDto.MobileNumber && u.Id != employeeDto.Id);
            if (existingPhone == null) return NotFound("Mobile Number already Exist ");
            var existingPan = _unitOfWork.Employee.FirstOrDefault(u => u.PanNumber == employeeDto.PanNumber && u.Id != employeeDto.Id);
            if (existingPan == null) return NotFound("PAN Number already Exist ");
            var employee = _mapper.Map<Employee>(employeeDto);

            var existingEmployee = await _unitOfWork.Employee.GetById(employeeDto.Id);
            if (existingEmployee == null)
            {
                return BadRequest("Employee not found");
            }

            existingEmployee.Name = existingEmployee.Name != employeeDto.Name ? employeeDto.Name : existingEmployee.Name;
            existingEmployee.Salary = existingEmployee.Salary != (decimal)employeeDto.Salary ? (decimal)employeeDto.Salary : existingEmployee.Salary;
            existingEmployee.CityId = existingEmployee.CityId != employeeDto.CityId ? employeeDto.CityId : existingEmployee.CityId;
            existingEmployee.Gender = existingEmployee.Gender != employeeDto.Gender?  employeeDto.Gender: existingEmployee.Gender;
            existingEmployee.Dob = existingEmployee.Dob != employeeDto.Dob ? employeeDto.Dob : existingEmployee.Dob;
            existingEmployee.JoiningDate = existingEmployee.JoiningDate != employeeDto.JoiningDate ? employeeDto.JoiningDate : existingEmployee.JoiningDate;
            existingEmployee.Email = existingEmployee.Email != employeeDto.Email ? employeeDto.Email : existingEmployee.Email;
            existingEmployee.MobileNumber = existingEmployee.MobileNumber != employeeDto.MobileNumber ? employeeDto.MobileNumber : existingEmployee.MobileNumber;
            existingEmployee.PanNumber = existingEmployee.PanNumber != employeeDto.PanNumber ? employeeDto.PanNumber : existingEmployee.PanNumber;
            existingEmployee.RelievingDate = existingEmployee.RelievingDate != employeeDto.RelievingDate ? employeeDto.RelievingDate : existingEmployee.RelievingDate;
            existingEmployee.UpdatedDate = DateTime.Now;
            _unitOfWork.Employee.Update(existingEmployee);
            _unitOfWork.save();

            return Ok(employeeDto);
        }

        [HttpPost("CheckEmailExists")]
        public async Task <IActionResult> CheckEmailExists(string email, int employeeId)
        {
            var existingEmployee =await _unitOfWork.Employee.FirstOrDefault(u => u.Email == email && u.Id != employeeId);
            if (existingEmployee == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("CheckMobilelExists")]
        public async Task< IActionResult> CheckMobileExists(string Mobile,int employeeId)
        {
            var existingEmployee = await _unitOfWork.Employee.FirstOrDefault(u => u.MobileNumber == Mobile && u.Id != employeeId);
            if (existingEmployee == null)
            {
                return BadRequest();
            }

            return Ok();
        }

    }

}
