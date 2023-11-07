using Microsoft.AspNetCore.Mvc;
using TibberRobotService.Data;
using TibberRobotService.Models;
using TibberRobotService.Services;

namespace TibberRobotService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RobotController : ControllerBase
    {
        private readonly IRobotService _robotService;

        private readonly ApplicationDbContext _context;
        public RobotController(IRobotService robotService, ApplicationDbContext context)
        {
            _robotService = robotService;
            _context = context;
        
        }

        [HttpPost("/tibber-developer-test/enter-path")]
        public async Task<IActionResult> EnterPath([FromBody] PathInputModel startCommand)
        {
            if (!ModelState.IsValid)
            {   
                // Return bad request if the input is not valid
                return BadRequest(ModelState);
            }
            var execution = _robotService.CalculatePathAndClean(startCommand);
            // Assuming a method that saves the execution to the database and returns it
            _context.Executions.Add(execution); 
            await _context.SaveChangesAsync();

            return Ok(execution);
        }
    }
}
