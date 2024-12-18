using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bugtracker_backend_net.Data;
using bugtracker_backend_net.Data.Models;
using bugtracker_backend_net.Data.DataTransferObjects;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;

namespace bugtracker_backend_net.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly BugtrackerDbContext _context;

        public ProjectsController(BugtrackerDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Users)
                .AsNoTracking()
                .ToListAsync();

            var projectResponses = projects.Select(project => new ProjectResponseWithUsersDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt.ToString("dd MMM, yyyy hh:mm tt"),
                UpdatedAt = project.UpdatedAt.ToString("dd MMM, yyyy hh:mm tt")
            }).ToList();

            return Ok(projectResponses);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetProject(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.Users)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { message = "Project not found." });
            }

            var projectResponse = new ProjectResponseWithUsersDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt.ToString("dd MMM, yyyy hh:mm tt"),
                UpdatedAt = project.UpdatedAt.ToString("dd MMM, yyyy hh:mm tt")
            };


            return Ok(projectResponse);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(Guid id, [FromBody] ProjectUpdateDto projectDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var project = await _context.Projects
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound($"Project with {id} not found.");
            }

            if (projectDto.Users.Length > 0)
            {
                foreach (var newUserId in projectDto.Users)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == newUserId);

                    if (user != null)
                    {
                        if (!project.Users.Any(u => u.Id == newUserId))
                        {
                            project.Users.Add(user);
                        }
                    }
                    else
                    {
                        return NotFound($"User with ID {userId} not found.");
                    }
                }
            }

            if (projectDto.Name != null) project.Name = projectDto.Name;
            if (projectDto.Description != null) project.Description = projectDto.Description;

            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var projectResponse = new ProjectResponseWithUsersDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt.ToString("dd MMM, yyyy hh:mm tt"),
                UpdatedAt = project.UpdatedAt.ToString("dd MMM, yyyy hh:mm tt")
            };

            return Ok(projectResponse);
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> PostProject([FromBody] ProjectDto projectDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }


            var existingProject = await _context.Projects
              .FirstOrDefaultAsync(p => p.Name == projectDto.Name);

            if (existingProject != null)
            {
                return Conflict(new { message = "A project with the same name already exists." });
            }


            var users = await _context.Users
                .Where(u => projectDto.Users.Contains(u.Id))
                .ToListAsync();

            if (users.Count != projectDto.Users.Length)
            {
                return BadRequest("Some users were not found.");
            }

            var project = new Data.Models.Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                Users = users
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectResponse = new ProjectResponseWithUsersDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt.ToString("MMM dd, yyyy hh:mm tt"),
                UpdatedAt = project.UpdatedAt.ToString("MMM dd, yyyy hh:mm tt")
            };



            return CreatedAtAction("GetProject", new { id = project.Id }, projectResponse);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }


            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{projectId}/users")]
        public async Task<IActionResult> AddUserToProject(Guid projectId, [FromBody] UserIdDto userIdDto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }


            var project = await _context.Projects.Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == projectId);


            if (project == null) return NotFound();


            var user = await _context.Users.FindAsync(userIdDto.Id);
            if (user == null) return NotFound();


            if (project.Users.Any(u => u.Id == userIdDto.Id))
                return BadRequest("User is already added to the project.");

            project.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpDelete("{projectId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromProject(Guid projectId, Guid userId)
        {
            var project = await _context.Projects.Include(p => p.Users)
                    .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return NotFound();

            var user = project.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("User not found in the project.");

            project.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        private bool ProjectExists(Guid id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
