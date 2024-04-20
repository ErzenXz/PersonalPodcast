using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PersonalPodcast.Services;
using PersonalPodcast.Models.Security;
using System.ComponentModel.DataAnnotations;

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

            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");


            user.Id = 0;
            user.Email = email;
            user.Password = passwordHashed;
            user.FirstLogin = DateTime.Now;
            user.LastLogin = DateTime.Now;
            user.ConnectingIp = HttpContext.Connection.RemoteIpAddress.ToString();
            user.Birthdate = userRequest.Birthdate;
            user.Role = userRequest.Role;
            user.Username = userRequest.Username;
            user.FullName = userRequest.FullName;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            user.TokenVersion = 1;

            // Save user to database
            _dBContext.Users.Add(user);
            await _dBContext.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully!", Code = 4 });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRequest userRequest)
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


            // Check if the user ip is blocked
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var ipBlock = _dBContext.ipMitigations.Where(i => i.IpAddress == ip).OrderByDescending(i => i.BlockedUntil).FirstOrDefault();

            if (ipBlock != null)
            {
                   if (ipBlock.BlockedUntil > DateTime.UtcNow)
                {
                    return Unauthorized(new { Message = "Our system has detected multiple login attempts from your IP address, which is a violation of our Terms of Service. As a result, access from your IP has been temporarily blocked for 10 minutes. This measure helps protect our platform from unauthorized access and ensures a secure environment for all users.", Code = 105 });
                }
            }

            // Check if user exists in database
            var user = _dBContext.Users.FirstOrDefault(u => u.Email == userRequest.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }


            if (!BCrypt.Net.BCrypt.Verify(userRequest.Password, user.Password))
            {

                // Add failed login attempt to database

                var userId = _dBContext.Users.FirstOrDefault(u => u.Email == userRequest.Email).Id;

                DateTime currentTime = DateTime.Now;
                DateTime oneHourAgo = currentTime.AddMinutes(-20).AddMilliseconds(-currentTime.Millisecond);

                var failedLoginAttempts = _dBContext.accountSecurity
                    .Where(a => a.UserId == userId && a.IpAddress == ip && a.LastFailedLogin >= oneHourAgo)
                    .OrderByDescending(a => a.LastFailedLogin)
                    .FirstOrDefault();




                if (failedLoginAttempts != null)
                {
                    failedLoginAttempts.FailedLoginAttempts += 1;
                    failedLoginAttempts.LastFailedLogin = DateTime.UtcNow;
                    _dBContext.accountSecurity.Update(failedLoginAttempts);
                    await _dBContext.SaveChangesAsync();

                    if (failedLoginAttempts.FailedLoginAttempts > 5)
                    {
                        var ipMitigation = new IpMitigations();
                        ipMitigation.IpAddress = ip;
                        ipMitigation.BlockedUntil = DateTime.UtcNow.AddMinutes(10);


                        // Remove all other failed login attempts from the same IP
                        _dBContext.ipMitigations.RemoveRange(_dBContext.ipMitigations.Where(a => a.IpAddress == ip));

                        _dBContext.ipMitigations.Add(ipMitigation);
                        await _dBContext.SaveChangesAsync();



                        return Unauthorized(new { Message = "Our system has detected multiple login attempts from your IP address, which is a violation of our Terms of Service. As a result, access from your IP has been temporarily blocked for 10 minutes. This measure helps protect our platform from unauthorized access and ensures a secure environment for all users.", Code = 105 });
                    }

                }
                else
                {
                    AccountSecurity accountSecurity = new AccountSecurity();
                    accountSecurity.UserId = userId;
                    accountSecurity.IpAddress = ip;
                    accountSecurity.FailedLoginAttempts = 1;
                    accountSecurity.LastFailedLogin = DateTime.UtcNow;
                    _dBContext.accountSecurity.Add(accountSecurity);
                    await _dBContext.SaveChangesAsync();
                }

                return BadRequest(new { Message = "Invalid password.", Code = 100 });
            }


            string accessToken = createAccessToken(user);

            if (accessToken != null)
            {

                if(user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow;
                    _dBContext.Users.Update(user);
                    await _dBContext.SaveChangesAsync();
                }

                // Generate new refresh token
                string newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                // Update the user with the new refresh token, expiry time and new token version
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.TokenVersion += 1;

                var oldConIP = user.ConnectingIp;
                user.ConnectingIp = HttpContext.Connection.RemoteIpAddress.ToString();


                _dBContext.Users.Update(user);
                await _dBContext.SaveChangesAsync();


                SetCookies(newRefreshToken);

                if(oldConIP != HttpContext.Connection.RemoteIpAddress.ToString())
                {
                    SecureMail.SendEmail("noreply@erzen.tk", "erzen.krasniqi04@gmail.com", "New Login from New IP", @"<p style=""font-size: 16px; color: #FF0000; font-weight: bold;"">⚠️ WARNING: SECURITY ALERT!</p>
<p style=""font-size: 14px;"">Dear User,</p>
<p style=""font-size: 14px;"">We regret to inform you that your account has been accessed from a <span style=""color: #FF0000;"">new, unauthorized IP address</span>. This may indicate a <span style=""color: #FF0000;"">security breach</span>.</p>
<p style=""font-size: 14px;"">If this login was not authorized by you, we urge you to <span style=""color: #FF0000;"">immediately change your password</span> by visiting <a href=""https://example.com/change-password"">this link</a>.</p>
<p style=""font-size: 14px;"">For your safety, do not ignore this message. If you believe your account has been compromised, <span style=""color: #FF0000;"">contact our support team</span> immediately.</p>
<p style=""font-size: 14px;"">Thank you for your attention to this urgent matter.</p>
<p style=""font-size: 14px;"">Sincerely,</p>
<p style=""font-size: 14px;"">PersonalPodcasts</p>
");
                }


                return Ok(new { Message = "User logged in successfully!", Code = 8, AccessToken = accessToken, newRefreshToken });
            }
            else
            {
                return BadRequest(new { Message = "Error creating access token.", Code = 9 });
            }

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "No refresh token found.", Code = 10 });
            }
            

            var user = _dBContext.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { Message = "Invalid refresh token or refresh token has expired.", Code = 11 });
            }

            // Generate new access token
            string newAccessToken = createAccessToken(user);

            // Generate new refresh token
            string newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // Update the user with the new refresh token and expiry time
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            SetCookies(newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "No refresh token found.", Code = 10 });
            }

            var user = _dBContext.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token.", Code = 11 });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;

            _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            SetCookies("");

            return Ok(new { Message = "User logged out successfully!", Code = 12 });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(UserRequest userRequest)
        {
            if (userRequest.Email == null)
            {
                return BadRequest(new { Message = "Email is required.", Code = 1 });
            }

            if (userRequest.Email.Length < 5 || userRequest.Email.Length > 100)
            {
                return BadRequest(new { Message = "Email must be between 5 and 100 characters.", Code = 2 });
            }

            // Check if user exists in database
            var user = _dBContext.Users.FirstOrDefault(u => u.Email == userRequest.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            // Generate new password
            string newPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8));

            // Hash the new password
            string newPasswordHashed = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Update the user with the new password
            user.Password = newPasswordHashed;

            _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            // Send email with new password

            var response = SecureMail.SendEmail("njnana2017@gmail.com", "erzen.krasniqi04@gmail.com", "New Password", "<h1>Hello!</h1><br>You have requested to reset your password in PersonalPodcast.<br>Here is your new password: <strong>" + newPassword + "</strong><br>Thanks!");

            if (!response)
            {
                return BadRequest(new { Message = "Error sending email. ", Code = 14 });
            }


            return Ok(new { Message = "New password sent to email. ", Code = 13 });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (changePasswordRequest.Email == null || changePasswordRequest.OldPassword == null || changePasswordRequest.NewPassword == null)
            {
                return BadRequest(new { Message = "Email, old password and new password are required.", Code = 1 });
            }

            if (changePasswordRequest.Email.Length < 5 || changePasswordRequest.Email.Length > 100)
            {
                return BadRequest(new { Message = "Email must be between 5 and 100 characters.", Code = 2 });
            }

            if (changePasswordRequest.OldPassword.Length < 8 || changePasswordRequest.OldPassword.Length > 100)
            {
                return BadRequest(new { Message = "Old password must be between 8 and 100 characters.", Code = 3 });
            }

            if (changePasswordRequest.NewPassword.Length < 8 || changePasswordRequest.NewPassword.Length > 100)
            {
                return BadRequest(new { Message = "New password must be between 8 and 100 characters.", Code = 4 });
            }

            // Check if user exists in database
            var user = _dBContext.Users.FirstOrDefault(u => u.Email == changePasswordRequest.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found.", Code = 101 });
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, user.Password))
            {
                return BadRequest(new { Message = "Invalid old password.", Code = 100 });
            }

            // Hash the new password
            string newPasswordHashed = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);

            // Update the user with the new password
            user.Password = newPasswordHashed;

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            user.TokenVersion += 1;

            _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            SetCookies(refreshToken);

            return Ok(new { Message = "Password changed successfully!", Code = 15 });
        }


        [HttpGet("info")]
        public async Task<IActionResult> GetUser()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized(new { Message = "No refresh token found.", Code = 10 });
            }

            var user = _dBContext.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token.", Code = 11 });
            }

            return Ok(new { user.FullName,user.Username, user.Email, user.Role });
        }


        private string createAccessToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.Username),
                new Claim(ClaimTypes.Email, user?.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("TokenVersion", user.TokenVersion.ToString()),
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSecurity:Secret").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creds
            };


            var jwt = new JwtSecurityTokenHandler();
            var token = jwt.CreateToken(tokenDescriptor);
            var accessToken = jwt.WriteToken(token);


            return accessToken;
        }

        private void SetCookies(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }

}
