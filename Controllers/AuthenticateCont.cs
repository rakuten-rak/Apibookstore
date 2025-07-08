using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Apibookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _authService.RegisterUserAsync(username, password);
            if (user == null)
            {
                return BadRequest("User already exists");
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _authService.LoginUserAsync(username, password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            // Here you would typically generate a JWT token and return it
            // For simplicity, we are returning the user object
            var token = CreateToken(user);
            return Ok(new { token });
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
                claims : claims,
                expires:DateTime.Now.AddHours(2),
                signingCredentials: creds
            );
            // return "GeneratedTokenForUser"; // Replace with actual token generation logic
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
