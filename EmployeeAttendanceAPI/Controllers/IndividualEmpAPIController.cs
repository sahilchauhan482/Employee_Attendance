using AutoMapper;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndividualEmpAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        public IndividualEmpAPIController(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [HttpPost("MarkIn")]
        public async Task<IActionResult> MarkIn([FromBody] AttendanceDTO attendanceDto)
        {
            try
            {
                if (attendanceDto == null) return BadRequest();

                var attend =await _unitOfWork.Attendance.FirstOrDefault(x => x.EmployeeId == attendanceDto.EmployeeId && x.AttendanceDate == DateTime.Now.Date);

                if (attend != null)
                {
                    
                    attend.InTime = DateTime.Now;
                    attend.Duration = "hh:mm";
                    attend.Leave = false;
                    attend.Absent = false;
                }
                else
                {
                    
                    var attendance = _mapper.Map<Attendance>(attendanceDto);
                    attendance.CreatedDate = DateTime.Now;
                    attendance.Absent = false;
                    attendance.Leave = false;
                    attendance.Duration = "hh:mm";
                    attendance.InTime = DateTime.Now;
                    attendance.AttendanceDate = DateTime.Now.Date;

                    await _unitOfWork.Attendance.Add(attendance);
                }

                _unitOfWork.save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAttendance")]

       public async Task<IActionResult> GetAttendance (int empid)
        {
            var empattendance=await _unitOfWork.Attendance.FirstOrDefault(x=>x.EmployeeId== empid&& x.AttendanceDate==DateTime.Now.Date);
            if (empattendance == null) return NotFound();
            var mapdata= _mapper.Map<AttendanceDTO>(empattendance);
            if (mapdata == null) return BadRequest();
            return Ok(mapdata);
        }

        [HttpPost("Markout")]
        public async Task<IActionResult> Markout([FromBody] AttendanceDTO attendanceDto)
        {
            try
            {
                if (attendanceDto == null) return BadRequest();

                var attend = await _unitOfWork.Attendance.FirstOrDefault(x => x.EmployeeId == attendanceDto.EmployeeId && x.AttendanceDate == DateTime.Now.Date);

                if (attend != null)
                {
                    attend.OutTime = DateTime.Now;
                }
                else
                {
                    var attendance = _mapper.Map<Attendance>(attendanceDto);
                    attendance.OutTime = DateTime.Now;
                    attendance.AttendanceDate = DateTime.Now.Date;
                    await _unitOfWork.Attendance.Add(attendance);
                }
                _unitOfWork.save();


                var email =await _unitOfWork.Employee.FirstOrDefault(x=>x.Id==attendanceDto.EmployeeId);
                if (email == null) return NotFound();
                var Empdetail = await _unitOfWork.Attendance.FirstOrDefault(x=>x.EmployeeId==attendanceDto.EmployeeId && x.AttendanceDate==DateTime.Now.Date);
                string subject = $"Attendance report of {DateTime.Now.Date:dd/MM/yyyy} ";
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine($"Dear {email.Name},<br><br>");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("Please find your attendance report for today:<br>");
                messageBuilder.AppendLine($"Mark-In Time - {Empdetail.InTime}<br>");
                messageBuilder.AppendLine($"Mark-Out Time - {Empdetail.OutTime}<br>");
                messageBuilder.AppendLine($"Working Duration - {Empdetail.OutTime-Empdetail.InTime}<br><br>");
                messageBuilder.AppendLine($"Thanks & regards  <br>");
                messageBuilder.AppendLine($"HR,Softwiz Infotech");
                await _emailSender.SendEmailAsync(email.Email, subject, messageBuilder.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
