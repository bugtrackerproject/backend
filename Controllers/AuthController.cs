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
    public class AuthController : ControllerBase {

        private readonly BugtrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AuthController(BugtrackerDbContext context, IOptions<JwtSettings> jwtSettings, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtSettings = jwtSettings;
            _passwordHasher = passwordHasher;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

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

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenObject = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshTokenObject);
            await _context.SaveChangesAsync();



            Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                token = accessToken,
                email = user.Email,
                name = user.Name,
                id = user.Id,
                role = user.Role.ToString()

            });

        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser([FromBody] UserDto userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            if (userDto.Password.Length < 3)
            {
                return BadRequest("Password must be at least 3 characters");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                return Conflict("User with this email already exists.");
            }

            var passwordHash = _passwordHasher.HashPassword(null, userDto.Password);


            var user = new User
            {
                Email = userDto.Email,
                Name = userDto.Name,
                PasswordHash = passwordHash,
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created(uri: $"/api/users/{user.Id}", value: new { email = user.Email, name = user.Name, role = user.Role });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token is missing." });
            }


            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            var user = await _context.Users.FindAsync(storedRefreshToken.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            var newAccessToken = GenerateAccessToken(user);

            var newRefreshToken = GenerateRefreshToken();

            _context.RefreshTokens.Remove(storedRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                accessToken = newAccessToken
            });
        }

        private string GenerateAccessToken(User user)
        {
            var jwtSettings = _jwtSettings.Value;

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
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
