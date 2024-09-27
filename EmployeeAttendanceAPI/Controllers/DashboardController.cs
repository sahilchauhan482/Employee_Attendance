using AutoMapper;
using EmployeeAPI.Repository;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DashboardController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("Get/{date}")]
        public async Task<IActionResult>Get(DateTime date)
        {
            var CurrentMonthData =await _unitOfWork.Employee.GetAll(x => x.JoiningDate.Month == date.Month&& x.JoiningDate.Day >=date.Day && x.IsActive);
            if (CurrentMonthData == null) return NotFound("CurrentMonthData doesn't exist");
            {
                var mappedCurrentMonthData = _mapper.Map<IEnumerable<EmployeeDto>>(CurrentMonthData);
                return Ok (mappedCurrentMonthData);
            }
        }

        [HttpGet("Birthday/{date}")]
        public async Task<IActionResult> Birthday(DateTime date)
        {
            var CurrentMonthData = await _unitOfWork.Employee.GetAll(x => x.Dob.Month == date.Month && x.Dob.Day >= date.Day && x.IsActive);
            if (CurrentMonthData == null) return NotFound("CurrentMonthData doesn't exist");
            {
                var mappedCurrentMonthData = _mapper.Map<IEnumerable<EmployeeDto>>(CurrentMonthData);
                return Ok(mappedCurrentMonthData);
            }
        }
    }
}
