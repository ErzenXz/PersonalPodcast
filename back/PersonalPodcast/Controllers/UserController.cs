using Microsoft.AspNetCore.Mvc;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getUserById")]
        public User GetUserById(long id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
            
        }
        
        [HttpGet("getUserByUsername")]
        public User GetUserByUsername(string username)
        {
            return new User
            {
                Id = 1,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpGet("getUsersByEmail")]
        public List<User> GetUsersByEmail(string email)
        {
            return new List<User>();
        }

        [HttpGet("getUsersByFullName")]
        public List<User> GetUsersByFullName(string fullName)
        {
            return new List<User>();
        }

        [HttpGet("getAllUsers")]
        public List<User> GetAllUsers()
        {
            return new List<User>();
        }

        [HttpPost("createUser")]
        public User CreateUser(User user)
        {
            return user;
        }

        [HttpPut("updateUser")]
        public User UpdateUser(int id)
        {
            return new User
            
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updatePassword")]
        public User UpdateUserPassword(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updateLastLogin")]
        public User UpdateUserLastLogin(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updateConnectingIp")]
        public User UpdateUserConnectingIp(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updateBirthdate")]
        public User UpdateUserBirthdate(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updateUsername")]
        public User UpdateUserUsername(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpPatch("updateFullName")]
        public User UpdateUserFullName(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpDelete("deleteUserById")]
        public User DeleteUserById(int id)
        {
            return new User
            {
                Id = id,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }

        [HttpDelete("deleteUserByUsername")]
        public User DeleteUserByUsername(string username)
        {
            return new User
            {
                Id = 1,
                Username = "username",
                FullName = "fullName",
                Email = "email",
                Password = "password",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "connectingIp",
                Birthdate = DateTime.Now
            };
        }
        
    }
}
