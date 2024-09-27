using EmployeeCommon.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeWeb.ClientFactory
{
    public class ClientService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _client.BaseAddress = new Uri(SD.APIBaseUrl);
            ProcessToken();
        }

        private void ProcessToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var data = httpContext.Request.Cookies["userData"];
                if (data != null)
                {
                    var tokenObject = JsonDocument.Parse(data).RootElement;
                    var tokenValue = tokenObject.GetProperty("token").GetString();
                    var expiration = GetTokenExpiration(tokenValue);

                    if (expiration > DateTime.Now)
                    {
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                    }
                    else
                    {
                        httpContext.Response.Redirect("/Login/Index");
                    }
                }
                else
                {
                    httpContext.Response.Redirect("/Login/Index");
                }
            }
        }

        private DateTime GetTokenExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenObject = tokenHandler.ReadJwtToken(token);
            var validTo = tokenObject.ValidTo; 
            var localValidTo = TimeZoneInfo.ConvertTimeFromUtc(validTo, TimeZoneInfo.Local); // Convert to local time
            return localValidTo;
        }
        public async Task<HttpResponseMessage> Get(string url)
        {
            return await _client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> Put(string url, StringContent stringContent)
        {
            return await _client.PutAsync(url, stringContent);
        }

        public async Task<HttpResponseMessage> Post(string url, StringContent stringContent)
        {
            return await _client.PostAsync(url, stringContent);
        }

        public async Task<HttpResponseMessage> Delete(string url)
        {
            return await _client.DeleteAsync(url);
        }
    }
}
