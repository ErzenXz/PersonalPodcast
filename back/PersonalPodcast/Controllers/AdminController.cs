using Microsoft.AspNetCore.Mvc;
using PersonalPodcast.Data;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public AdminController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

    }
}
