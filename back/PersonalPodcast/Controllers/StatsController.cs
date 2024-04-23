using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("stats")]
    public class StatsController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public StatsController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet, Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                DateTime tenMinutesAfter = DateTime.UtcNow.AddMinutes(10);

                var stats = new Stats
                {
                    Podcasts = await _dBContext.Podcasts.CountAsync(),
                    Episodes = await _dBContext.Episodes.CountAsync(),
                    Users = await _dBContext.Users.CountAsync(),
                    IpsBlocked = await _dBContext.ipMitigations.CountAsync(),
                    IpsCurrenltyBlocked = await _dBContext.ipMitigations
                    .Where(i => i.BlockedUntil >= tenMinutesAfter && i.BlockedUntil <= DateTime.UtcNow)
                    .CountAsync()
            };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching stats");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}
