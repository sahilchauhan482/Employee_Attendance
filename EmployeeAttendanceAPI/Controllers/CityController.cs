using AutoMapper;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class CityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CityController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        [HttpGet("cities/{stateId}")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities(int stateId)
        {
            var cities = await _unitOfWork.City.GetAll(c => c.StateId == stateId);
            if (cities != null)
            {
                var citiesDto = _mapper.Map<IEnumerable<CityDto>>(cities);
                return Ok(citiesDto);
            }
            return BadRequest();
        }
    }
}
