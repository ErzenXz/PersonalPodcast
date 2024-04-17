using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public UserController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }


        
        [HttpGet("getUserById")]
        public async Task<User> GetUserById(long id)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogInformation("User request for " + id + " has failed!");

                return null;
            }

            _logger.LogInformation("User request for " + id + " has been successful!");

            return user;
            
        }
        
        [HttpGet("getUserByUsername")]
        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogInformation("User request for " + username + " has failed!");

                return null;
            }

            _logger.LogInformation("User request for " + username + " has been successful!");
            return user;
        }

        [HttpGet("getUserByEmail")]
        public async Task<User> GetUsersByEmail(string email)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null)
            {
                   return null;
            }
 
            return user;

        }

        [HttpGet("getUsersByFullName")]
        public async IAsyncEnumerable<User> GetUsersByFullName(string fullname)
        {
            var users = _dBContext.Users.Where(u => u.Username == fullname).AsAsyncEnumerable();
            await foreach (var user in users)
            {
                yield return user;
            }

        }

        [HttpGet("getAllUsers")]
        public async IAsyncEnumerable<User> GetAllUsers(int page)
        {
            var users = _dBContext.Users.Skip(page * 10).Take(10).AsAsyncEnumerable();
            await foreach (var user in users)
            {
                yield return user;
            }
        }


        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser(long id, string username, string fullname, string email, string password, string newPassword, DateTime lastlogin, DateTime firstlogin, string conIP, DateTime birthday)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            user.Username = username;
            user.FullName = fullname;
            user.Email = email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.FirstLogin = firstlogin;
            user.LastLogin = lastlogin;
            user.ConnectingIp = conIP;
            user.Birthdate = birthday;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "User updated successfully.", Code = 105 });
        }

        [HttpPatch("updatePassword")]
        public async Task<IActionResult> UpdateUserPassword(long id, string oldPassword, string newPassword)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });

            }

            if(newPassword.Length < 8 || newPassword.Length > 100)
            {
                return BadRequest(new { Message = "Password must be between 8 and 100 characters.", Code = 3 });
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                return BadRequest(new { Message = "Old password is incorrect.", Code = 106 });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Password updated successfully.", Code = 107 });
        }
            
            

        [HttpPatch("updateLastLogin")]
        public async Task<IActionResult> UpdateUserLastLogin(long id, string password)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            user.LastLogin = DateTime.Now;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Last login updated successfully.", Code = 108 });
        }

        [HttpPatch("updateConnectingIp")]
        public async Task<IActionResult> UpdateUserConnectingIp(long id, string password)
        {
           var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            user.ConnectingIp = HttpContext.Connection.RemoteIpAddress.ToString();
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Connecting IP updated successfully.", Code = 109 });
        }

        [HttpPatch("updateBirthdate")]
        public async Task<IActionResult> UpdateUserBirthdate(long id, DateTime newBirthDay, string password)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            if (newBirthDay == null)
            {
                return BadRequest(new { Message = "Birthdate is required.", Code = 115 });
            }

            user.Birthdate = newBirthDay;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Birthdate updated successfully.", Code = 110 });
        }

        [HttpPatch("updateUsername")]
        public async Task<IActionResult> UpdateUserUsername(long id, string newUsername, string password)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (_dBContext.Users.Any(u => u.Username == newUsername))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 104 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            user.Username = newUsername;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Username updated successfully.", Code = 111 });

        }

        [HttpPatch("updateFullName")]
        public async Task<IActionResult> UpdateUserFullName(long id, string newName, string password)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect.", Code = 106 });
            }

            user.FullName = newName;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Full name updated successfully.", Code = 112 });
        }

        [HttpDelete("deleteUserById"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserById(long id)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }
            _dBContext.Users.Remove(user);
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "User deleted successfully.", Code = 113 });
        }

        [HttpDelete("deleteUserByUsername"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserByUsername(string username)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }
            _dBContext.Users.Remove(user);
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "User deleted successfully.", Code = 113 });
        }
       

    }
}
