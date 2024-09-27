using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace EmployeeWeb
{
    public static class SD
    {
        public static string APIBaseUrl = "https://localhost:44320/api";
        public static string EmployeeApiPath = APIBaseUrl + "/Employee";
        public static string AttendanceApiPath = APIBaseUrl + "/attendance";
        public static string AttendanceByMonthApiPath = APIBaseUrl + "/attendance/GetAttendanceByMonth";
        public static string CountriesApiPath = APIBaseUrl + "/Country/countries";
        public static string StatesApiPath = APIBaseUrl + "/State/states";
        public static string CityApiPath = APIBaseUrl + "/City/cities";
        public static string LoginApiPath = APIBaseUrl + "/User/login";
        public static string RegisterApiPath = APIBaseUrl + "/User/Register";
        public static string RefreshTokenApi = APIBaseUrl + "/User/RefreshToken";
        public static string CurrentMonthaniversary = APIBaseUrl + "/Dashboard/Get";
        public static string Birthdates = APIBaseUrl + "/Dashboard/Birthday";

       
    }
}
