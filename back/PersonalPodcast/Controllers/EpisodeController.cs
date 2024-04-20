using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("episodes")]
    public class EpisodeController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public EpisodeController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EpisodeRequest request)
        {
            try
            {
                var episode = new Episode
                {
                    Id = request.Id,
                    PodcastId = request.PodcastId,
                    Title = request.Title,
                    Description = request.Description,
                    Tags = request.Tags,
                    PosterImg = request.PosterImg,
                    AudioFileUrl = request.AudioFileUrl,
                    VideoFileUrl = request.VideoFileUrl,
                    Length = request.Length,
                    Views = request.Views,
                    PublisherId = request.PublisherId
                };

                _dBContext.Episodes.Add(episode);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Episode created successfully: {EpisodeId}", episode.Id);

                return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetEpisode(long id)
        {
            try
            {
                var episode = await _dBContext.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning("Episode with Id {EpisodeId} not found", id);
                    return NotFound($"Episode with Id {id} not found.");
                }
                var episodeResponse = new EpisodeResponse
                {
                    Id = episode.Id,
                    PodcastId = episode.PodcastId,
                    Title = episode.Title,
                    Description = episode.Description,
                    Tags = episode.Tags,
                    PosterImg = episode.PosterImg,
                    AudioFileUrl = episode.AudioFileUrl,
                    VideoFileUrl = episode.VideoFileUrl,
                    Length = episode.Length,
                    Views = episode.Views,
                    PublisherId = episode.PublisherId

                };
                return Ok(episodeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEpisodes(int page)
        {
            if (page < 1)
            {
                return BadRequest("Invalid page number.");
            }

            try
            {
                var episodes = await _dBContext.Episodes
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .Select(c => new EpisodeResponse
                    {
                        Id = c.Id,
                        PodcastId = c.PodcastId,
                        Title = c.Title,
                        Description = c.Description,
                        Tags = c.Tags,
                        PosterImg = c.PosterImg,
                        AudioFileUrl = c.AudioFileUrl,
                        VideoFileUrl = c.VideoFileUrl,
                        Length = c.Length,
                        Views = c.Views,
                        PublisherId = c.PublisherId

                    })
                    .ToListAsync();

                return Ok(episodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all episodes");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] EpisodeRequest request)
        {
            try
            {
                var episode = await _dBContext.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning("Podcast with Id {PodcastId} not found", id);
                    return NotFound($"Podcast with Id {id} not found.");
                }
                episode.Id = request.Id;
                episode.PodcastId = request.PodcastId;
                episode.Title = request.Title;
                episode.Description = request.Description;
                episode.Tags = request.Tags;
                episode.PosterImg = request.PosterImg;
                episode.AudioFileUrl = request.AudioFileUrl;
                episode.VideoFileUrl = request.VideoFileUrl;
                episode.Length = request.Length;
                episode.Views = request.Views;
                episode.PublisherId = request.PublisherId;

                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Episode with Id {EpisodeId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var episode = await _dBContext.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning("Episode with Id {EpisodeId} not found", id);
                    return NotFound($"Episode with Id {id} not found.");
                }

                _dBContext.Episodes.Remove(episode);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Episode with Id {EpisodeId} deleted successfully", id);

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
