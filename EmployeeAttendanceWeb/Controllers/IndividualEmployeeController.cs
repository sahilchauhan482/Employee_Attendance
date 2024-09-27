using EmployeeCommon.DTOs;
using EmployeeWeb.ClientFactory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EmployeeWeb.Controllers
{

    public class IndividualEmployeeController : Controller
    {
        private readonly ClientService _clientService;
        public IndividualEmployeeController(ClientService clientService)
        {
            _clientService = clientService;
        }
        public async Task<IActionResult> Index()
        {

            var employeeCookie = Request.Cookies["Employee"];
            var employeeData = JObject.Parse(employeeCookie);
            var result = employeeData["result"];
            if (result == null || result.Type == JTokenType.Null) return View("Index");
            {
                var empid = employeeData["result"]["id"].Value<int>();
                var attendance = await GetAttendance(empid);
                return View(attendance);
            }

            
        }

        private async Task<AttendanceDTO> GetAttendance(int empid)
        {
            string apiUrl = $"https://localhost:44320/api/IndividualEmpAPI/GetAttendance?empid={empid}";
            var response = await _clientService.Get(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<AttendanceDTO>(content);
                return attendance;
            }
            else
            {
                return null;
            }
        }




        [HttpPost]
        [Route("IndividualEmployee/PunchIn")]

        public async Task<IActionResult> PunchIn([FromBody] AttendanceDTO attendanceDto)
        {

            var jsonContent = JsonConvert.SerializeObject(attendanceDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _clientService.Post("https://localhost:44320/api/IndividualEmpAPI/MarkIn", stringContent);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }
        }
        [HttpPost]
        [Route("IndividualEmployee/PunchOut")]
        public async Task<IActionResult> PunchOut([FromBody] AttendanceDTO attendanceDto)
        {

            var jsonContent = JsonConvert.SerializeObject(attendanceDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _clientService.Post("https://localhost:44320/api/IndividualEmpAPI/Markout", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }
        }
    }
}
