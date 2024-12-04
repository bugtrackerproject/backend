using bugtracker_backend_net.Data;
using bugtracker_backend_net.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            return await _context.Tickets.ToListAsync();
        }

        // GET api/<TicketsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TicketsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
