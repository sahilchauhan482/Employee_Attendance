using EmployeeCommon.DTOs;
using EmployeeWeb.ClientFactory;
using EmployeeWeb.Models;
using EmployeeWeb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace EmployeeWeb.Controllers
{
   
    public class EmployeeController : Controller
    {
        private readonly ClientService _clientService;
        public EmployeeController(ClientService clientService)
        {
            
            _clientService = clientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            var response = await _clientService.Get(SD.EmployeeApiPath);

            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<EmployeeDto>>(content);
                return Json(new { data });
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return RedirectToAction("Index","Login");
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }

        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var employeeViewModel = new EmployeeViewModel();
            employeeViewModel.CountryList = await GetCountriesAsync();

            
                var employee = await GetEmployeeAsync(id);

                
                    var states = await GetStatesAsync(employee.CountryId);
                    var cities = await GetCitiesAsync(employee.StateId);
                    employeeViewModel.employees = employee;
                    employeeViewModel.StateList = states;
                    employeeViewModel.CityList = cities;
                    return View(employeeViewModel);
                
            }

            [HttpGet]
        public async Task<IActionResult> CreateUpdate(int employeeId)
        {
            var employeeViewModel = new EmployeeViewModel();
            employeeViewModel.CountryList = await GetCountriesAsync();

            if (employeeId > 0)
            {
                var employee = await GetEmployeeAsync(employeeId);

                if (employee != null)
                {
                    var states = await GetStatesAsync(employee.CountryId);
                    var cities = await GetCitiesAsync(employee.StateId);
                    employeeViewModel.employees = employee;
                    employeeViewModel.StateList = states;
                    employeeViewModel.CityList = cities;
                    return PartialView(employeeViewModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return PartialView(employeeViewModel);
            }
        }

        [HttpPost]

        public async Task<IActionResult> CreateUpdate(EmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(model);
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response;

                    if (model.Id > 0)
                    {
                        model.UpdatedDate = DateTime.Now;
                        response = await _clientService.Put(SD.EmployeeApiPath, stringContent);
                    }
                    else
                    {
                        response = await _clientService.Post(SD.EmployeeApiPath, stringContent);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                var employeeViewModel = new EmployeeViewModel();
                return PartialView(employeeViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStatesByCountryId(int countryId)
        {

            var states = await GetStatesAsync(countryId);
            if (states != null)
            {
                return Ok(states);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByStateId(int stateId)
        {

            var cities = await GetCitiesAsync(stateId);
            if (cities != null)
            {
                return Ok(cities);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            
            HttpResponseMessage response = await _clientService.Delete($"{SD.EmployeeApiPath}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        #region Private Methods
        private async Task<EmployeeDto> GetEmployeeAsync(int employeeId)
        {
            
            HttpResponseMessage response = await _clientService.Get($"{SD.EmployeeApiPath}/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<EmployeeDto>(content);
                return employee;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) RedirectToAction("Index", "Login");
                return null;
            }
        }
        private async Task<IEnumerable<CountryDto>> GetCountriesAsync()
        {
            
            HttpResponseMessage response = await _clientService.Get(SD.CountriesApiPath);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<IEnumerable<CountryDto>>(content);
                return countries;
            }
            else
            {
                return null;
            }
        }
        private async Task<IEnumerable<StateDto>> GetStatesAsync(int countryId)
        {
            
            HttpResponseMessage response = await _clientService.Get($"{SD.StatesApiPath}/{countryId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var states = JsonConvert.DeserializeObject<IEnumerable<StateDto>>(content);
                return states;
            }
            else
            {
                return null;
            }
        }
        private async Task<IEnumerable<CityDto>> GetCitiesAsync(int stateId)
        {
            
            HttpResponseMessage response = await _clientService.Get($"{SD.CityApiPath}/{stateId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cities = JsonConvert.DeserializeObject<IEnumerable<CityDto>>(content);
                return cities;
            }
            else
            {
                return null;
            }
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> EmailCheck(string email,int empid)
        {
            
            var jsonContent = JsonConvert.SerializeObject(email);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _clientService.Post($"{SD.EmployeeApiPath}/CheckEmailExists?email={email}&employeeId={empid}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }

        }
        

        [HttpPost]
        public async Task<IActionResult> PhoneNoCheck(string phonenumber,int empid)
        {
            
            var jsonContent = JsonConvert.SerializeObject(phonenumber);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _clientService.Post($"{SD.EmployeeApiPath}/CheckMobilelExists?mobile={phonenumber}&employeeId={empid}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }

        }
    }
}