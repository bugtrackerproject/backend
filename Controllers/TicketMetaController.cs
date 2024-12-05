using bugtracker_backend_net.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace bugtracker_backend_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketMetaController : ControllerBase
    {
        // GET api/ticketmeta/types
        [HttpGet("types")]
        public ActionResult<IEnumerable<string>> GetTicketTypes()
        {
            // Format enum values with spaces
            var types = Enum.GetValues(typeof(TicketType))
                            .Cast<TicketType>()
                            .Select(t => AddSpacesToCamelCase(t.ToString()))
                            .ToList();
            return Ok(types);
        }

        // GET api/ticketmeta/priorities
        [HttpGet("priorities")]
        public ActionResult<IEnumerable<string>> GetTicketPriorities()
        {
            // Format enum values with spaces
            var priorities = Enum.GetValues(typeof(TicketPriority))
                                 .Cast<TicketPriority>()
                                 .Select(p => AddSpacesToCamelCase(p.ToString()))
                                 .ToList();
            return Ok(priorities);
        }

        // GET api/ticketmeta/statuses
        [HttpGet("statuses")]
        public ActionResult<IEnumerable<string>> GetTicketStatuses()
        {
            // Format enum values with spaces
            var statuses = Enum.GetValues(typeof(TicketStatus))
                               .Cast<TicketStatus>()
                               .Select(s => AddSpacesToCamelCase(s.ToString()))
                               .ToList();
            return Ok(statuses);
        }

        // Helper method to add spaces between camel case words
        private string AddSpacesToCamelCase(string input)
        {
            return Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");
        }
    }
}
