using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.Models;
using PersonalPodcast.Models.Security;
using System.Data;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("admin"), Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public AdminController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("all",Name = "admins/all")]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await _dBContext.Users.Where(u => u.Role == "Admin" || u.Role == "SuperAdmin").ToListAsync();

                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching admins");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        

        [HttpDelete("remove/{id}",Name ="admins/remove/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                var admin = await _dBContext.Users.FirstOrDefaultAsync(u => u.Id == id && (u.Role == "Admin" || u.Role == "SuperAdmin"));

                if (admin == null)
                {
                    return NotFound();
                }

                admin.Role = "User";

                _dBContext.Users.Update(admin);
                await _dBContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting admin");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("create",Name ="admins/create")]
        public async Task<IActionResult> CreateAdmin([FromBody] User user)
        {
            try
            {
                user.Role = "Admin";
                await _dBContext.Users.AddAsync(user);
                await _dBContext.SaveChangesAsync();

                return Ok("Admin created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating admin");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Path user

        [HttpGet("users/all", Name ="users/all")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _dBContext.Users.Where(u => u.Role == "User").ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("users/{id}",Name ="users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                var userToUpdate = await _dBContext.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == "User");

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                userToUpdate.Email = user.Email;
                userToUpdate.Password = user.Password;

                await _dBContext.SaveChangesAsync();

                return Ok(userToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Path ipMitigation

        [HttpGet("security/blocked", Name = "security/blocked")]
        public async Task<IActionResult> GetIpMitigations(int page)
        {
            try
            {
                if (page < 0)
                {
                    return BadRequest();
                }

                var ipMitigations = await _dBContext.ipMitigations.Skip(page * 10).Take(10).ToListAsync();

                return Ok(ipMitigations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ipMitigations");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("security/blocked/{id}", Name = "security/blocked/{id}")]
        public async Task<IActionResult> GetIpMitigation(int id)
        {
            try
            {
                var ipMitigation = await _dBContext.ipMitigations.FindAsync(id);

                if (ipMitigation == null)
                {
                    return NotFound();
                }

                return Ok(ipMitigation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ipMitigation");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("security/recheck/{id}", Name = "security/recheck/{id}")]
        public async Task<IActionResult> UpdateIpMitigation(int id, [FromBody] IpMitigations ipMitigation)
        {
            try
            {
                var ipMitigationToUpdate = await _dBContext.ipMitigations.FindAsync(id);

                if (ipMitigationToUpdate == null)
                {
                    return NotFound();
                }

                ipMitigationToUpdate.BlockedUntil = ipMitigation.BlockedUntil;

                await _dBContext.SaveChangesAsync();

                return Ok(ipMitigationToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ipMitigation");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("security/unblock/{id}", Name = "security/unblock/{id}")]
        public async Task<IActionResult> DeleteIpMitigation(int id)
        {
            try
            {
                var ipMitigation = await _dBContext.ipMitigations.FindAsync(id);

                if (ipMitigation == null)
                {
                    return NotFound();
                }

                _dBContext.ipMitigations.Remove(ipMitigation);
                await _dBContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ipMitigation");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("security/block", Name = "security/block")]
        public async Task<IActionResult> CreateIpMitigation([FromBody] IpMitigations ipMitigation)
        {
            try
            {
                await _dBContext.ipMitigations.AddAsync(ipMitigation);
                await _dBContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIpMitigation), new { id = ipMitigation.Id }, ipMitigation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ipMitigation");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }



    }
}
