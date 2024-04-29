using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PersonalPodcast.Data;
using PersonalPodcast.Models;
using PersonalPodcast.Services;
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


        
        [HttpGet("id/{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<User> GetUserById(long id)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogInformation("User request for " + id + " has failed!");

                return null;
            }

            _logger.LogInformation("User request for " + id + " has been successful!");

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return user;
            
        }
        
        [HttpGet("username/{username}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogInformation("User request for " + username + " has failed!");

                return null;
            }

            _logger.LogInformation("User request for " + username + " has been successful!");

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");


            return user;
        }

        [HttpGet("email/{email}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<User> GetUsersByEmail(string email)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null)
            {
               return null;
            }

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");
 
            return user;

        }

        [HttpGet("fullname/{fullname}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async IAsyncEnumerable<User> GetUsersByFullName(string fullname)
        {
            var users = _dBContext.Users.Where(u => u.Username == fullname).AsAsyncEnumerable();
            await foreach (var user in users)
            {
                yield return user;
            }

        }

        [HttpGet("all"), Authorize(Roles = "Admin,SuperAdmin")]
        public async IAsyncEnumerable<User> GetAllUsers(int page = 1, string? range = null)
        {
            // Parse the range query parameter

            var queryParams = ParameterParser.ParseRangeAndSort(range, "sort");

            var users = _dBContext.Users.Skip((queryParams.Page - 1) * 10).Take(10).AsAsyncEnumerable();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users {queryParams.Page * 10}-{(queryParams.Page * 10) + 10}/{_dBContext.Users.Count()}");


            await foreach (var user in users)
            {
                yield return user;
            }
        }


        [HttpPut("update"),Authorize(Roles ="User,Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUser(string username, string fullname, string email, string newPassword, DateTime lastlogin, DateTime firstlogin, string conIP, DateTime birthday)
        {

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            if (_dBContext.Users.Any(u => u.Username == username))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 8 });
            }

            if (_dBContext.Users.Any(u => u.Email == email))
            {
                return BadRequest(new { Message = "Email already in use.", Code = 7 });
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

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "User updated successfully.", Code = 79 });
        }    

        [HttpPatch("update/last-login")]
        public async Task<IActionResult> UpdateUserLastLogin()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            var user1 = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


            if (user1 == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            user1.LastLogin = DateTime.UtcNow;
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "Last login updated successfully.", Code = 80 });
        }

        [HttpPatch("update/connecting-ip")]
        public async Task<IActionResult> UpdateUserConnectingIp()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            var user1 = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user1 == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }


            user1.ConnectingIp = HttpContext.Connection.RemoteIpAddress.ToString();
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "Connecting IP updated successfully.", Code = 81 });
        }

        [HttpPatch("update/birthdate")]
        public async Task<IActionResult> UpdateUserBirthdate(DateTime newBirthDay)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            // Check if the new birthdate is valid
            if (newBirthDay > DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Invalid birthdate.", Code = 9 });
            }

            if (newBirthDay == user.Birthdate)
            {
                return BadRequest(new { Message = "Birthdate is the same as the current one.", Code = 15 });
            }

            user.Birthdate = newBirthDay;
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "Birthdate updated successfully.", Code = 84 });
        }

        [HttpPatch("update/username")]
        public async Task<IActionResult> UpdateUserUsername(string newUsername)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }

            if (newUsername == null)
            {
                   return BadRequest(new { Message = "Username cannot be empty.", Code = 12 });
            }

            if (_dBContext.Users.Any(u => u.Username == newUsername))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 8 });
            }

            user.Username = newUsername;
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "Username updated successfully.", Code = 82 });

        }

        [HttpPatch("update/fullname")]
        public async Task<IActionResult> UpdateUserFullName(string newName)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "Unauthorized to perform this request.", Code = 76 });
            }
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 36 });
            }

            if (newName == null)
            {
                return BadRequest(new { Message = "Full name cannot be empty.", Code = 16 });
            }

            user.FullName = newName;
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "Full name updated successfully.", Code = 83 });
        }

        [HttpDelete("delete/id/{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteUserById(long id)
        {
            var user = await _dBContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 36 });
            }
            _dBContext.Users.Remove(user);
            await _dBContext.SaveChangesAsync();
            return Ok(new { Message = "User deleted successfully.", Code = 400 });
        }

        [HttpDelete("delete/username/{username}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteUserByUsername(string username)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 36 });
            }
            _dBContext.Users.Remove(user);
            await _dBContext.SaveChangesAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"users 0-0/1");

            return Ok(new { Message = "User deleted successfully.", Code = 400 });
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
