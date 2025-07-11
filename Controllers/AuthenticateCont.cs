//using Apibookstore.DTOs;
using Apibookstore.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Apibookstore.Controllers
{
    public class RegisterDto
    {
     
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        //[Required]
        public string Email { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    [ApiController]
    [Route("api/auth")]
    public class AuthenticateCont : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    //return View();

        //}
        private readonly Service.AuthService _authService;
        private readonly IConfiguration? _configuration;
        public AuthenticateCont(Service.AuthService authService, IConfiguration? configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Username and password are required");
                }

                var user = await _authService.RegisterUserAsync(registerDto.Username, registerDto.Password);
                if (user == null)
                {
                    return BadRequest("User already exists");
                }

                return Ok(new { message = "Registration successful", userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.LoginUserAsync(loginDto.Username, loginDto.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                var token = CreateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string CreateToken(models.Booksusers user)
        {
            // This method should create a JWT token using the user's information
            // For simplicity, we are returning a placeholder string
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );
            // return "GeneratedTokenForUser"; 
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}








