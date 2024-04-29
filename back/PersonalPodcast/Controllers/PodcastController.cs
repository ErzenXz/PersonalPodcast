using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("podcasts")]
    public class PodcastController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public PodcastController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }
        [HttpPost, Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(PodcastRequest request)
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

                _logger.LogInformation($"Podcast created successfully: {podcast.Id}");

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts 1-1/1");

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
                    _logger.LogWarning($"Podcast with Id {id} not found");
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
                    PublisherId = podcast.PublisherId,
                    CreatedDate = podcast.CreatedDate,
                    LastUpdate = podcast.LastUpdate
                    
                };

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts 1-1/1");

                return Ok(podcastResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPodcasts(int page = 1, int perPage = 10)
        {
            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 11 });
            }

            try
            {
                var podcasts = await _dBContext.Podcasts
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
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
                        PublisherId = c.PublisherId,
                        CreatedDate = c.CreatedDate,
                        LastUpdate = c.LastUpdate
                    })
                    .ToListAsync();

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts {((page - 1) * perPage) + 1}-{page * perPage}/{podcasts.Count}");


                return Ok(podcasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all podcasts");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Get all episodes that belong to a podcast

        [HttpGet("{id}/episodes")]
        public async Task<IActionResult> GetPodcastEpisodes(long id, int page = 1, int perPage = 10)
        {
            try
            {
                if (page < 1)
                {
                    return BadRequest(new { Message = "Invalid page number.", Code = 11 });
                }

                var podcast = await _dBContext.Podcasts.FindAsync(id);
                if (podcast == null)
                {
                    _logger.LogWarning($"Podcast with Id {id} not found");
                    return NotFound(new { Message = $"Podcast with Id {id} not found.", Code = 60 });
                }

                var episodes = await _dBContext.Episodes
                    .Where(e => e.PodcastId == id)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(e => new EpisodeResponse
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Tags = e.Tags,
                        PosterImg = e.PosterImg,
                        AudioFileUrl = e.AudioFileUrl,
                        VideoFileUrl = e.VideoFileUrl,
                        Length = e.Length,
                        Views = e.Views,
                        PublisherId = e.PublisherId,
                        CreatedDate = e.CreatedDate,
                        LastUpdate = e.LastUpdate,
                        PodcastId = e.PodcastId

                    })
                    .ToListAsync();

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"episodes {((page - 1) * perPage) + 1}-{page * perPage}/{episodes.Count}");

                return Ok(episodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving podcast episodes");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Search for podcasts by title, description or tags

        [HttpGet("search")]
        public async Task<IActionResult> SearchPodcasts(string query, int page = 1, int perPage = 10, int[]? range = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { Message = "Search query cannot be empty.", Code = 19 });
            }

            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 11 });
            }

            try
            {

                // Add suport for range query

                if (range != null)
                {
                    perPage = range[1] - range[0] + 1;
                    page = range[0] / perPage + 1;
                }


                var podcasts = await _dBContext.Podcasts
                    .Where(p => p.Title.Contains(query) || p.Description.Contains(query) || p.Tags.Contains(query))
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(p => new PodcastResponse
                    {
                        id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        CategoryId = p.CategoryId,
                        Tags = p.Tags,
                        PosterImg = p.PosterImg,
                        AudioFileUrl = p.AudioFileUrl,
                        VideoFileUrl = p.VideoFileUrl,
                        PublisherId = p.PublisherId,
                        CreatedDate = p.CreatedDate,
                        LastUpdate = p.LastUpdate
                    })
                    .ToListAsync();

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts {((page - 1) * perPage) + 1}-{page * perPage}/{podcasts.Count}");

                return Ok(podcasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for podcasts");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(long id, [FromBody] PodcastRequest request)
        {
            try
            {
                var podcast = await _dBContext.Podcasts.FindAsync(id);
                if(podcast == null)
                {
                    _logger.LogWarning($"Podcast with Id {id} not found", id);
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

                _logger.LogInformation($"Podcast with Id {id} updated successfully", id);

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts 1-1/1");

                return Ok(new { Message = "Podcast updated successfully.", Code = 87 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
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

                _logger.LogInformation($"Podcast  with Id {id} deleted successfully", id);

                // Add Content-Range header

                HttpContext.Response.Headers.Append("Content-Range", $"podcasts 1-1/1");

                return Ok(new { Message = "Podcast deleted successfully.", Code = 88 });
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting podcast");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}
