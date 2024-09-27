using EmployeeCommon.DTOs;
using EmployeeWeb.ClientFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;

namespace EmployeeWeb.Controllers
{
    public class AttendanceMonthlyController : Controller
    {
        private readonly ClientService _clientService;
        public AttendanceMonthlyController(ClientService clientService)
        {
            _clientService = clientService;
        }
        public async Task<IActionResult>Index(DateTime date)
        {
            date = date == DateTime.MinValue ? DateTime.Now : date;
            var attendance =await GetAttendanceByMonth(DateTime.Now);
            if(attendance == null) return RedirectToAction("Index","Login");
            return View(attendance);

        }

        public async Task<IActionResult>GetByMonth(DateTime date)
        {
            var attendance = await GetAttendanceByMonth(date);
            return PartialView("_ByMonthATTPartail",attendance);
        }

        private async Task<IEnumerable<AttendanceDTO>>GetAttendanceByMonth(DateTime date)
        {
            string apiUrl = $"{SD.AttendanceByMonthApiPath}/{date:yyyy-MM-dd}";
            var response = await _clientService.Get(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<IEnumerable<AttendanceDTO>>(content);
                return attendance;
            }
            else
            {
                if(response.StatusCode== HttpStatusCode.Unauthorized) return null;
                return null;
            }
        }

        [HttpPost]
        [Route("AttendanceMonthly/MonthlyReport")]
        public async Task<IActionResult> MonthlyReport(DateTime date)
        {
            var jsonContent = JsonConvert.SerializeObject(date);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _clientService.Post($"https://localhost:44320/api/Attendance/DownloadAttendanceReport/{date:yyyy-MM-dd}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                return File(contentStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceMonthly.xlsx");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }
        }
    }
}
