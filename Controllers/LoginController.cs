using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bugtracker_backend_net.Data.DataTransferObjects;
using bugtracker_backend_net.Data.Models;
using Microsoft.AspNetCore.Identity;
using bugtracker_backend_net.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bugtracker_backend_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase {

        private readonly BugtrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public LoginController(BugtrackerDbContext context, IOptions<JwtSettings> jwtSettings, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtSettings = jwtSettings;
            _passwordHasher = passwordHasher;
        }


        [HttpPost()]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var jwtSettings = _jwtSettings.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var passwordCorrect = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (passwordCorrect == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
             };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = jwtToken,
                email = user.Email,
                name = user.Name,
                id = user.Id,
                role = user.Role.ToString()

            });

        }
    }
}
