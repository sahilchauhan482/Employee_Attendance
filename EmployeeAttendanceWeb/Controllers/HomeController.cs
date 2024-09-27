using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EmployeeWeb.Models;
using Microsoft.AspNetCore.Http;
using EmployeeWeb.ClientFactory;
using EmployeeCommon.DTOs;
using Newtonsoft.Json;
using EmployeeWeb.Models.ViewModel;

namespace EmployeeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClientService _clientService;

        public HomeController(ILogger<HomeController> logger, ClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {

            if (Request.Cookies["userData"] != null)
            {
                EmployeeViewModel model = new EmployeeViewModel();
                var Birthdays = await GetBirthdate(DateTime.Now);
                var Aniversaries = await GetAniversaryData(DateTime.Now);
                var attendance = await GetAttendanceByDateAsync(DateTime.Now);
                model.employeeBDto = Birthdays.ToList();
                model.employeesAniversariesDTOs = Aniversaries.ToList();
                if (attendance != null) { 
                model.EmployeeAttendance = attendance.ToList();
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        private async Task<IEnumerable<AttendanceDTO>> GetAttendanceByDateAsync(DateTime date)
        {
            var employees = await GetEmployeesAsync();
            var attendance = await GetAttendanceAsync(date);
            if (attendance != null) {
                var employeesIds = attendance.Select(x => x.EmployeeId).ToList();
            }
            return attendance;
        }


        private async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync()
        {
            var response = await _clientService.Get("https://localhost:44320/api/Employee");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<IEnumerable<EmployeeDto>>(content);
                return employees;
            }
            else
            {
                return null;
            }
        }
        private async Task<IEnumerable<AttendanceDTO>> GetAttendanceAsync(DateTime date)
        {

            string apiUrl = $"{SD.AttendanceApiPath}/Get/{date:yyyy-MM-dd}";
            var response = await _clientService.Get(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var attendance = JsonConvert.DeserializeObject<IEnumerable<AttendanceDTO>>(content);
                return attendance;
            }
            else
            {
                return null;
            }
        }
        private async Task<IEnumerable<EmployeesAniversariesDTO>> GetAniversaryData(DateTime date)
        {

            string apiUrl = $"{SD.CurrentMonthaniversary}/{date:yyyy-MM-dd}";
            var response = await _clientService.Get(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var workani = JsonConvert.DeserializeObject<IEnumerable<EmployeesAniversariesDTO>>(content);
                return workani;
            }
            else
            {
                return null;
            }
        }

        private async Task<IEnumerable<EmployeeBDto>> GetBirthdate(DateTime date)
        {

            string apiUrl = $"{SD.Birthdates}/{date:yyyy-MM-dd}";
            var response = await _clientService.Get(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var birthdays = JsonConvert.DeserializeObject<IEnumerable<EmployeeBDto>>(content);
                return birthdays;
            }
            else
            {
                return null;
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}