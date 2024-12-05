using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bugtracker_backend_net.Data;
using bugtracker_backend_net.Data.Models;
using bugtracker_backend_net.Data.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace bugtracker_backend_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BugtrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(BugtrackerDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                return NotFound();
            }


            return Ok(new
            {
                email = user.Email,
                name = user.Name,
                role = user.Role
            });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, [FromBody] UserDto userDto)
        {
            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            if (user.Email != userDto.Email)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userDto.Email);

                if (existingUser != null)
                {
                    return Conflict("User with this email already exists.");
                }
            }

            user.Email = userDto.Email;
            user.Name = userDto.Name;
            user.Role = userDto.Role;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(null, userDto.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                email = user.Email,
                name = user.Name,
                role = user.Role
            });
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                new
                {
                    email = user.Email,
                    name = user.Name,
                    role = user.Role
                }
            );
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
