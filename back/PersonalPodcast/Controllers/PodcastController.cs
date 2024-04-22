using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("podcast")]
    public class PodcastController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public PodcastController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]PodcastRequest request)
        {
            try
            {
                var podcast = new Podcast
                {
                    Title = request.Title,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    Tags = request.Tags,
                    PosterImg = request.PosterImg,
                    AudioFileUrl = request.AudioFileUrl,
                    VideoFileUrl = request.VideoFileUrl,
                    PublisherId = request.PublisherId
                };

                _dBContext.Podcasts.Add(podcast);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Podcast created successfully: {PodcastId}", podcast.Id);

                return CreatedAtAction(nameof(GetPodcast), new { id = podcast.Id }, podcast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPodcast(long id)
        {
            try
            {
                var podcast = await _dBContext.Podcasts.FindAsync(id);
                if (podcast == null)
                {
                    _logger.LogWarning("Podcast with Id {PodcastId} not found", id);
                    return NotFound(new { Message = $"Podcast with Id {id} not found.", Code = 60 });
                }
                var podcastResponse = new PodcastResponse
                {
                    id = podcast.Id,
                    Title = podcast.Title,
                    Description = podcast.Description,
                    CategoryId = podcast.CategoryId,
                    Tags = podcast.Tags,
                    PosterImg = podcast.PosterImg,
                    AudioFileUrl = podcast.AudioFileUrl,
                    VideoFileUrl = podcast.VideoFileUrl,
                    PublisherId = podcast.PublisherId
                    
                };
                return Ok(podcastResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPodcasts(int page)
        {
            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 55 });
            }

            try
            {
                var podcasts = await _dBContext.Podcasts
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .Select(c => new PodcastResponse
                    {
                        id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        CategoryId = c.CategoryId,
                        Tags = c.Tags,
                        PosterImg = c.PosterImg,
                        AudioFileUrl = c.AudioFileUrl,
                        VideoFileUrl = c.VideoFileUrl,
                        PublisherId = c.PublisherId


                    })
                    .ToListAsync();

                return Ok(podcasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all podcasts");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] PodcastRequest request)
        {
            try
            {
                var podcast = await _dBContext.Podcasts.FindAsync(id);
                if(podcast == null)
                {
                    _logger.LogWarning("Podcast with Id {PodcastId} not found", id);
                    return NotFound(new { Message = $"Podcast with Id {id} not found.", Code = 60 });
                }

                podcast.Title = request.Title;
                podcast.Description = request.Description;
                podcast.CategoryId = request.CategoryId;
                podcast.Tags = request.Tags;
                podcast.PosterImg = request.PosterImg;
                podcast.AudioFileUrl = request.AudioFileUrl;
                podcast.VideoFileUrl = request.VideoFileUrl;
                podcast.PublisherId = request.PublisherId;

                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Podcast with Id {PodcastId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var podcast = await _dBContext.Podcasts.FindAsync(id);
                if (podcast == null)
                {
                    _logger.LogWarning("Podcast with Id {PodcastId} not found", id);
                    return NotFound(new { Message = $"Podcast with Id {id} not found.", Code = 60 });
                }

                _dBContext.Podcasts.Remove(podcast);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Podcast  with Id {PodcastId} deleted successfully", id);

                return NoContent();
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}
