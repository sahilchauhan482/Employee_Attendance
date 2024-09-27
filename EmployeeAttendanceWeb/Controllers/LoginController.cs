using EmployeeCommon.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace EmployeeWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            if (userDto == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri(SD.APIBaseUrl);
            var jsonContent = JsonConvert.SerializeObject(userDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(SD.LoginApiPath,stringContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var data = JObject.Parse(content);
                var userData = data["employee"].ToString();
                var token = data["token"].ToString(); 
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var role = jsonToken?.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
               HttpContext.Session.SetString("Role", role);


                Response.Cookies.Append("Employee", userData, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                });

                Response.Cookies.Append("userData", content, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(60) 
                });
               
                return Ok(content);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (userDto == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri(SD.APIBaseUrl);
            var jsonContent = JsonConvert.SerializeObject(userDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(SD.RegisterApiPath, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }

        }

        [HttpPost]
        [Route("Login/RefreshToken")]
        public async Task<IActionResult>RefreshToken(string refreshtoken)
        {
            if (refreshtoken == null) return NotFound();
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri(SD.APIBaseUrl);
            var jsonContent = JsonConvert.SerializeObject(refreshtoken);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{SD.RefreshTokenApi}?refreshToken={refreshtoken}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                Response.Cookies.Append("userData", "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });

                Response.Cookies.Append("userData", content, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                });

                return Ok(content);

            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorMessage);
            }

        }

        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Append("userData", "", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });

            return Ok();
        }
    }
}
