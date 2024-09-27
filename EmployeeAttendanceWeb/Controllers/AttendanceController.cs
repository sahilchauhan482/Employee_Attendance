using EmployeeCommon.DTOs;
using EmployeeWeb.ClientFactory;
using EmployeeWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EmployeeWeb.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ClientService _clientService;
        public AttendanceController( ClientService clientService)
        {
            _clientService = clientService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new AttendanceViewModel();
            var attendance = await GetAttendanceByDateAsync(DateTime.Now);
            if(attendance == null) return RedirectToAction("Index","Login");
            model.EmployeeAttendance = attendance.ToList();
            return View(model);
        }

        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var attendance = await GetAttendanceByDateAsync(date);
            return PartialView("_AttendancePartial", attendance);
        }

        private async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync()
        {
            var response = await _clientService.Get(SD.EmployeeApiPath);

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

        private async Task<IEnumerable<AttendanceDTO>> GetAttendanceByDateAsync(DateTime date)
        {
            var employees = await GetEmployeesAsync();
            var attendance = await GetAttendanceAsync(date);
            if (attendance != null && attendance.Any())
            {
                var employeesIds = attendance.Select(x => x.EmployeeId).ToList();
                var allEmployeeId = employees.Select(x => x.Id).ToList();
                var missingEmployeeIds = allEmployeeId.Except(employeesIds);
                var employeeAttendance = missingEmployeeIds.Select(x => new AttendanceDTO
                {
                    EmployeeId = x,
                    EmployeeName = employees.First(q => q.Id == x).Name
                }).ToList();
                attendance = attendance.Concat(employeeAttendance).ToList();
            }
            else
            {
                if (employees != null)
                {
                    attendance = employees.Select(x => new AttendanceDTO
                    {
                        EmployeeId = x.Id,
                        EmployeeName = x.Name
                    }).ToList();
                }
            }

            return attendance;
        }

        [HttpPut]
        public async Task<IActionResult> Edit(List<AttendanceDTO> attendanceDTO)
        {            
            var jsonContent = JsonConvert.SerializeObject(attendanceDTO);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _clientService.Put(SD.AttendanceApiPath, stringContent);
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var employees = await GetEmployeesAsync();
            return View(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create(List<AttendanceDTO> attendanceDto)
        {
            
            var jsonContent = JsonConvert.SerializeObject(attendanceDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _clientService.Post(SD.AttendanceApiPath, stringContent);
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
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _clientService.Delete($"api/attendance/{id}");
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
