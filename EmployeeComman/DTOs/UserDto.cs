using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.DTOs
{
    public class UserDto
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
