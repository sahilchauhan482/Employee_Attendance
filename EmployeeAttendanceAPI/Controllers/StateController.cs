using AutoMapper;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StateController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        [HttpGet("states/{countryid}")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetStates(int countryid)
        {
            var states = await _unitOfWork.State.GetAll(c => c.CountryId == countryid);
            if (states != null)
            {
                var statesDto = _mapper.Map<IEnumerable<StateDto>>(states);
                return Ok(statesDto);
            }
            return BadRequest();
        }
    }
}
