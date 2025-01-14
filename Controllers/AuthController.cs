using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using restAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using restAPI.Repositories;

namespace restAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly List<User> users;
        
        public AuthController(IAuthRepository authRepository,IConfiguration configuration)
        {
            _configuration = configuration;
            _authRepository = authRepository;
        }

        // Endpoint logowania
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var users = await _authRepository.GetUsers();
            var user = users.FirstOrDefault(u => u.Id == loginDto.Username && u.Password == loginDto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user); 
            return Ok(new { user.Id,token, role = user.Role });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authRepository.AddRaportToAcceptByAdmin(registerDto);
            return Ok(result);
        }
        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount(long id)
        {
            var result = await _authRepository.ActivateAccount(id);
            return Ok(result);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])); 
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5078", // Backend URL
                audience: "http://localhost:3000", // Frontend URL
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
