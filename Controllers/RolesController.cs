using bugtracker_backend_net.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using bugtracker_backend_net.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace bugtracker_backend_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly BugtrackerDbContext _context;

        public RolesController(BugtrackerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAllRoles()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                            .Cast<UserRole>()
                            .Select(r => r.ToString())
                            .ToList();

            return Ok(roles);
        }
    }
}
