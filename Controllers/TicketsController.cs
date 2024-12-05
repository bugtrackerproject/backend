﻿using bugtracker_backend_net.Data;
using bugtracker_backend_net.Data.DataTransferObjects;
using bugtracker_backend_net.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bugtracker_backend_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly BugtrackerDbContext _context;

        public TicketsController(BugtrackerDbContext context)
        {
            _context = context;
        }

        // GET: api/<TicketsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Submitter)
              .AsNoTracking()
              .ToListAsync();

            var ticketResponses = tickets.Select(ticket => new TicketResponseDto
            {
                Id = ticket.Id,
                Name = ticket.Name,
                Description = ticket.Description,
                Status = ticket.Status,
                Type = ticket.Type,
                Priority = ticket.Priority,
                Project = ticket.Project?.Id ?? Guid.Empty,
                Assignee = ticket.Assignee?.Id ?? Guid.Empty,
                Submitter = ticket.Submitter?.Id ?? Guid.Empty,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            }).ToList();

            return Ok(ticketResponses);
        }

        // GET api/<TicketsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDto>> GetTicket(Guid id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Submitter)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found." });
            }

            var ticketResponse = new TicketResponseDto
            {
                Id = ticket.Id,
                Name = ticket.Name,
                Description = ticket.Description,
                Status = ticket.Status,
                Type = ticket.Type,
                Priority = ticket.Priority,
                Project = ticket.Project?.Id ?? Guid.Empty,
                Assignee = ticket.Assignee?.Id ?? Guid.Empty,
                Submitter = ticket.Submitter?.Id ?? Guid.Empty,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };


            return Ok(ticketResponse);
        }

        // POST api/<TicketsController>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TicketResponseDto>> PostTicket([FromBody] TicketDto ticketDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            Guid submitterId = Guid.Parse(userId);

            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == ticketDto.Project);

            var assignee = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == ticketDto.Assignee);

            var submitter = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == submitterId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found." });
            }

            if (submitter == null)
            {
                return NotFound(new { message = "Submitter not found." });
            }


            var ticket = new Ticket
            {
                Name = ticketDto.Name,
                Description = ticketDto.Description,
                Type = ticketDto.Type,
                Status = TicketStatus.ToDo,
                Priority = ticketDto.Priority,
                ProjectId = ticketDto.Project,
                AssigneeId = ticketDto.Assignee,
                SubmitterId = submitterId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var ticketResponse = new TicketResponseDto
            {
                Id = ticket.Id,
                Name = ticket.Name,
                Description = ticket.Description,
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Project = ticket.ProjectId,
                Assignee = ticket.AssigneeId,
                Submitter = ticket.SubmitterId,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };



            return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticketResponse);
        }


        // PUT api/<TicketsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TicketsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
