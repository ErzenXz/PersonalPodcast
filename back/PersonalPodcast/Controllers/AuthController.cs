using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonalPodcast.Data;
using PersonalPodcast.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalPodcast.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly DBContext _dBContext;

        public AuthController(IConfiguration configuration, DBContext dBContext) {
            _configuration = configuration;
            _dBContext = dBContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest userRequest)
        {
            if (userRequest.Email == null || userRequest.Password == null)
            {
                return BadRequest(new { Message = "Email and password are required.", Code = 1 });
            }

            if (userRequest.Email.Length < 5 || userRequest.Email.Length > 100)
            {
                return BadRequest(new { Message = "Email must be between 5 and 100 characters.", Code = 2 });
            }

            if (userRequest.Password.Length < 8 || userRequest.Password.Length > 100)
            {
                return BadRequest(new { Message = "Password must be between 8 and 100 characters.", Code = 3 });
            }

            if (_dBContext.Users.Any(u => u.Email == userRequest.Email))
            {
                return BadRequest(new { Message = "Email already in use.", Code = 102 });
            }

            if (userRequest.Birthdate == null)
            {
                return BadRequest(new { Message = "Birthdate is required.", Code = 103 });
            }

            if(_dBContext.Users.Any(u => u.Username == userRequest.Username))
            {
                return BadRequest(new { Message = "Username already in use.", Code = 104 });
            }

            string passwordHashed = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);
            string email = userRequest.Email;

            user.Id = 0;
            user.Email = email;
            user.Password = passwordHashed;
            user.FirstLogin = DateTime.Now;
            user.LastLogin = DateTime.Now;
            user.ConnectingIp = "";
            user.Birthdate = userRequest.Birthdate;
            user.Role = userRequest.Role;
            user.Username = userRequest.Username;
            user.FullName = userRequest.FullName;

            // Save user to database
            _dBContext.Users.Add(user);
            await _dBContext.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully!", Code = 4 });
        }

        [HttpPost("login")]
        public IActionResult Login(UserRequest userRequest)
        {
            if (userRequest.Email == null || userRequest.Password == null)
            {
                return BadRequest(new { Message = "Email and password are required.", Code = 1 });
            }

            if (userRequest.Email.Length < 5 || userRequest.Email.Length > 100)
            {
                return BadRequest(new { Message = "Email must be between 5 and 100 characters.", Code = 2 });
            }

            if (userRequest.Password.Length < 8 || userRequest.Password.Length > 100)
            {
                return BadRequest(new { Message = "Access Denied: You need to provide a password in order to login.", Code = 99 });
            }

            // Check if user exists in database
            User user = _dBContext.Users.FirstOrDefault(u => u.Email == userRequest.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }


            if (!BCrypt.Net.BCrypt.Verify(userRequest.Password, user.Password))
            {
                return BadRequest(new { Message = "Invalid password.", Code = 100 });
            }

            string accessToken = createAccessToken(user);
            if (accessToken != null)
            {
                return Ok(new { Message = "User logged in successfully!", Code = 8, AccessToken = accessToken });
            }
            else
            {
                return BadRequest(new { Message = "Error creating access token.", Code = 9 });
            }
        }

        private string createAccessToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.Username),
                new Claim(ClaimTypes.Email, user?.Email), 
                new Claim(ClaimTypes.Role, user.Role)
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSecurity:Secret").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                //issuer: _configuration.GetSection("AppSecurity:Issuer").Value,
                //audience: _configuration.GetSection("AppSecurity:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
