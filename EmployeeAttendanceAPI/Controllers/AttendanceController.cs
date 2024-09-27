using AutoMapper;
using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using EmployeeCommon.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AttendanceController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AttendanceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("Get/{date}")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> Get(DateTime date)
        {
            try
            {
                var datewise = await _unitOfWork.Attendance.GetAll(x => x.AttendanceDate == date, IncludeProperties: "Employee");
                var attendanceDtos = _mapper.Map<IEnumerable<AttendanceDTO>>(datewise);
                return Ok(attendanceDtos);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("GetAttendanceByMonth/{date}")]
        public async Task<IActionResult> GetAttendanceByMonth(DateTime date)
        {
            var MonthlyAttendance = await _unitOfWork.Attendance.GetAll(x => x.AttendanceDate.Month == date.Month, IncludeProperties: "Employee");

            var MonthlyAttendanceMapping = _mapper.Map<IEnumerable<AttendanceDTO>>(MonthlyAttendance);
            return Ok(MonthlyAttendanceMapping);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDTO>> Get(int id)
        {
            var attendance = await _unitOfWork.Attendance.FirstOrDefault(x => x.Id == id, IncludeProperties: "Employee");
            if (attendance == null)
            {
                return NotFound();
            }
            var attendanceDto = _mapper.Map<AttendanceDTO>(attendance);
            return Ok(attendanceDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<AttendanceDTO> attendanceDto)
        {
            try
            {
                if (attendanceDto == null) return BadRequest();

                var attendance = _mapper.Map<List<Attendance>>(attendanceDto);
                foreach (var item in attendance)
                {
                    await _unitOfWork.Attendance.Add(item);
                    item.CreatedDate = DateTime.Now;

                }
                _unitOfWork.save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }




        [HttpPut]
        public async Task<IActionResult> Put([FromBody] List<AttendanceDTO> attendanceDto)
        {

            var attendance = _mapper.Map<IEnumerable<Attendance>>(attendanceDto);
            foreach (var item in attendance)
            {
                if (item.Id > 0)
                {
                    item.UpdatedDate = DateTime.Now;
                    _unitOfWork.Attendance.Update(item);
                }
                else
                {
                    item.CreatedDate = DateTime.Now;
                    await _unitOfWork.Attendance.Add(item);
                }
            }
            _unitOfWork.save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(AttendanceDTO attendanceDTO)
        {
            var attendance = await _unitOfWork.Attendance.GetById(attendanceDTO.Id);
            if (attendance == null)
            {
                return NotFound();
            }

            attendanceDTO.IsDeleted = true;


            _unitOfWork.Attendance.Update(attendance);
            _unitOfWork.save();

            return NoContent();
        }

        [HttpPost("DownloadAttendanceReport/{date}")]
        public async Task<IActionResult> DownloadAttendanceReport(DateTime date)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                var data = await GetDataForMonth(date);
                var groupedData = data.GroupBy(x => x.EmployeeName);
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    var allDates = DateUtility.GetAllDatesInMonth(data.Min(x => x.AttendanceDate));
                    var column = 5;
                    worksheet.Cells[1, 1].Value = "Employee Name";
                    worksheet.Cells[1, 2].Value = "Present Count";
                    worksheet.Cells[1, 3].Value = "Leave Count";
                    worksheet.Cells[1, 4].Value = "Absent Count";
                    foreach (var day in allDates)
                    {
                        worksheet.Cells[1, column].Value = day.ToString("dd MMM");
                        column++;
                    }
                    var row = 2;
                    foreach (var group in groupedData)
                    {
                        column = 2;
                        worksheet.Cells[row, 1].Value = group.Key;
                        int presentCount = 0, leaveCount = 0, absentCount = 0;
                        foreach (var day in allDates)
                        {
                            var attendance = group.FirstOrDefault(x => x.AttendanceDate.Date == day.Date);
                            if (attendance != null)
                            {
                                var status = "";
                                if (attendance.InTime != DateTime.MinValue && attendance.OutTime != DateTime.MinValue)
                                {
                                    TimeSpan duration = attendance.OutTime - attendance.InTime;
                                    status = duration.ToString("hh\\:mm");
                                    presentCount++;
                                }
                                else if (attendance.Leave)
                                {
                                    status = "L";
                                    leaveCount++;
                                }
                                else if (attendance.Absent)
                                {
                                    status = "A";
                                    absentCount++;
                                }
                                
                                var dateColumnIndex = allDates.IndexOf(day) + 5; 
                                worksheet.Cells[row, dateColumnIndex].Value = status;
                            }
                            column++;
                        }
                       
                        worksheet.Cells[row, 2].Value = presentCount;
                        worksheet.Cells[row, 3].Value = leaveCount;
                        worksheet.Cells[row, 4].Value = absentCount;
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "AttendanceMonthly.xlsx";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    return File(stream, contentType, fileName);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task<IEnumerable<AttendanceDTO>> GetDataForMonth(DateTime date)
        {
            var MonthlyAttendance = await _unitOfWork.Attendance.GetAll(x => x.AttendanceDate.Month == date.Month, IncludeProperties: "Employee");

            var MonthlyAttendanceMapping = _mapper.Map<IEnumerable<AttendanceDTO>>(MonthlyAttendance);
            return MonthlyAttendanceMapping;
        }



    }
}
