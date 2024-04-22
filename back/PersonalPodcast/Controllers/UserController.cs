using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PersonalPodcast.Data;
using PersonalPodcast.Models;
using System.Security.Cryptography;

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


        
        [HttpGet("getUserById"), Authorize(Roles = "Admin,SuperAdmin")]
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
        
        [HttpGet("getUserByUsername"), Authorize(Roles = "Admin,SuperAdmin")]
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

        [HttpGet("getUserByEmail"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<User> GetUsersByEmail(string email)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null)
            {
                   return null;
            }
 
            return user;

        }

        [HttpGet("getUsersByFullName"), Authorize(Roles = "Admin,SuperAdmin")]
        public async IAsyncEnumerable<User> GetUsersByFullName(string fullname)
        {
            var users = _dBContext.Users.Where(u => u.Username == fullname).AsAsyncEnumerable();
            await foreach (var user in users)
            {
                yield return user;
            }

        }

        [HttpGet("getAllUsers"), Authorize(Roles = "Admin,SuperAdmin")]
        public async IAsyncEnumerable<User> GetAllUsers(int page)
        {
            var users = _dBContext.Users.Skip(page * 10).Take(10).AsAsyncEnumerable();
            await foreach (var user in users)
            {
                yield return user;
            }
        }


        [HttpPut("updateUser"),Authorize(Roles ="User,Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUser(string username, string fullname, string email, string newPassword, DateTime lastlogin, DateTime firstlogin, string conIP, DateTime birthday)
        {


            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            if (_dBContext.Users.Any(u => u.Username == username))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 104 });
            }

            if (_dBContext.Users.Any(u => u.Email == email))
            {
                return BadRequest(new { Message = "Email already in use.", Code = 105 });
            }

            user.Username = username;
            user.FullName = fullname;
            user.Email = email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.FirstLogin = firstlogin;
            user.LastLogin = lastlogin;
            user.ConnectingIp = conIP;
            user.Birthdate = birthday;
            // Generatge a new refresh token
            user.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            await _dBContext.SaveChangesAsync();

            SetCookies(refreshToken);

            return Ok(new { Message = "User updated successfully.", Code = 150 });
        }    

        [HttpPatch("updateLastLogin")]
        public async Task<IActionResult> UpdateUserLastLogin()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            var user1 = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


            if (user1 == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            user1.LastLogin = DateTime.UtcNow;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Last login updated successfully.", Code = 108 });
        }

        [HttpPatch("updateConnectingIp")]
        public async Task<IActionResult> UpdateUserConnectingIp()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            var user1 = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user1 == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }


            user1.ConnectingIp = HttpContext.Connection.RemoteIpAddress.ToString();
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Connecting IP updated successfully.", Code = 109 });
        }

        [HttpPatch("updateBirthdate")]
        public async Task<IActionResult> UpdateUserBirthdate(DateTime newBirthDay)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            // Check if the new birthdate is valid
            if (newBirthDay > DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Invalid birthdate.", Code = 114 });
            }

            if (newBirthDay == user.Birthdate)
            {
                return BadRequest(new { Message = "Birthdate is the same as the current one.", Code = 115 });
            }

            user.Birthdate = newBirthDay;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Birthdate updated successfully.", Code = 110 });
        }

        [HttpPatch("updateUsername")]
        public async Task<IActionResult> UpdateUserUsername(string newUsername)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }

            if (newUsername == null)
            {
                   return BadRequest(new { Message = "Username cannot be empty.", Code = 103 });
            }

            if (_dBContext.Users.Any(u => u.Username == newUsername))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 104 });
            }

            user.Username = newUsername;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Username updated successfully.", Code = 111 });

        }

        [HttpPatch("updateFullName")]
        public async Task<IActionResult> UpdateUserFullName(string newName)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 107 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (newName == null)
            {
                return BadRequest(new { Message = "Full name cannot be empty.", Code = 106 });
            }

            user.FullName = newName;
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "Full name updated successfully.", Code = 112 });
        }

        [HttpDelete("deleteUserById"), Authorize(Roles = "Admin,SuperAdmin")]
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

        [HttpDelete("deleteUserByUsername"), Authorize(Roles = "Admin,SuperAdmin")]
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

        private void SetCookies(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


    }
}
