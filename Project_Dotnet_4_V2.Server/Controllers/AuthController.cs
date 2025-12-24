using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Projet_Dotnet_4.Models;
using Projet_Dotnet_4.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Projet_Dotnet_4.DTO.Auth;
using Projet_Dotnet_4.Data;

namespace Projet_Dotnet_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthController(IConfiguration configuration, IUserService userService, ApplicationDbContext applicationDbContext)
        {
            _configuration = configuration;
            _userService = userService;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO userLogin)
        {
            try
            {
                
                if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrEmpty(userLogin.Password))
                {
                    return BadRequest(new { message = "Username and password required" });
                }


                var user = await _userService.GetByUsername(userLogin.Username);

                if (user == null)
                {
                    return Unauthorized(new { message = " Invalid identifiant " });
                }

                
                if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Identifiants invalides" });
                }

                
                

                
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                   
                };

                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                
                if (string.IsNullOrEmpty(registerDto.Username) ||
                    string.IsNullOrEmpty(registerDto.Password) ||
                    string.IsNullOrEmpty(registerDto.Email))
                {
                    return BadRequest(new { message = "All fields are required" });
                }

                
                if (await _userService.UsernameExists(registerDto.Username))
                {
                    return BadRequest(new { message = "This username already exists" });
                }

                if (await _userService.EmailExists(registerDto.Email))
                {
                    return BadRequest(new { message = "This mail already exists" });
                }

                
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    Role = registerDto.Role ?? "Client",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                _userService.Create(user);

                return Ok(new { message = "Sucess Register my homie !" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server", error = ex.Message });
            }
        }
    }
}