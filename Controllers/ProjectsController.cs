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

namespace bugtracker_backend_net.Controllers
{
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

            var projectResponses = projects.Select(project => new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
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

            var projectResponse = new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList()
            };


            return Ok(projectResponse);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(Guid id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> PostProject([FromBody] ProjectDto projectDto)
        {
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

            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                Users = users
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectResponse = new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => u.Id).ToList(),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };



            return CreatedAtAction("GetProject", new { id = project.Id }, projectResponse);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(Guid id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
