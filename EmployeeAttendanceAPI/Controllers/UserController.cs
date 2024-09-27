using AutoMapper;
using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;
using EmployeeCommon.DTOs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> _signInManager;


        UserDto user = new UserDto();

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration,
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
            Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                if (userDto == null) { return NotFound("Username and password are required."); }
                string hashedPassword = userDto.Password;
                var newUser = new User
                {
                    UserName = userDto.UserName,
                };

                var result = await _userManager.CreateAsync(newUser, hashedPassword);
                if (result.Succeeded)
                {

                    var roleExists = await _roleManager.RoleExistsAsync(userDto.Role);
                    if (roleExists)
                    {
                        await _userManager.AddToRoleAsync(newUser, userDto.Role);
                    }

                    return Ok(newUser);
                }
                else
                {
                    return BadRequest("Failed to create user.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            try
            {
                var user = await _unitOfWork.User.FirstOrDefault(u => u.UserName == userDto.UserName);
                var user1 = await _userManager.FindByNameAsync(userDto.UserName);
                if (user == null)
                {
                    return BadRequest("Invalid username or password.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user1, userDto.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var employee = FindEmployee(user1.UserName);
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
                    _unitOfWork.User.Update(user);
                    _unitOfWork.save();
                    var token = GenerateToken(user.UserName, await _userManager.GetRolesAsync(user1));

                    return Ok(new { Token = token, User = user, RefreshToken = refreshToken, Employee= employee });
                }
                else
                {
                    return BadRequest("Invalid username or password.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<Employee> FindEmployee(string user)
        {
            if (user == null) return null;
            var employee = await _unitOfWork.Employee.FirstOrDefault(x => x.Email == user);
            if (employee == null) return null;
            return employee ;
        }

        private string GenerateToken(string userName, IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userName),
    };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                _configuration["JWT:ValidIssuer"],
                _configuration["JWT:ValidAudience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);
                var expiration = DateTime.UtcNow.AddDays(7);
                user.RefreshTokenExpiration = expiration;
                return refreshToken;
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is missing.");
            }
            var user = await _unitOfWork.User.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return BadRequest("Invalid refresh token.");
            }

            if (user.RefreshTokenExpiration < DateTime.UtcNow)
            {
                return BadRequest("Refresh token has expired.");
            }
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _unitOfWork.User.Update(user);
            _unitOfWork.save();
            var user1 = await _userManager.FindByNameAsync(user.UserName);
            var token = GenerateToken(user.UserName, await _userManager.GetRolesAsync(user1));
            return Ok(new { Token = token, User = user, RefreshToken = newRefreshToken });
        }
    }
}
