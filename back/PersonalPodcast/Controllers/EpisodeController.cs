using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using PersonalPodcast.Services;

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

        [HttpPost, Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(EpisodeRequest request)
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
                    PublisherId = request.PublisherId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow
                };

                _dBContext.Episodes.Add(episode);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Episode created successfully: {EpisodeId}", episode.Id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"episodes 0-1/1");

                return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating episode");
                return StatusCode(500, "An error occurred while processing the request. " + ex.InnerException?.Message);
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
                    return NotFound(new { Message = $"Episode with Id {id} not found.", Code = 59 });

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
                    PublisherId = episode.PublisherId,
                    CreatedDate = episode.CreatedDate,
                    LastUpdate = episode.LastUpdate

                };

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"episodes 0-1/1");

                return Ok(episodeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEpisodes(int page = 1, int perPage = 10, string? range = null)
        {
            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 11 });
            }

            // Parse the range query parameter

            var queryParams = ParameterParser.ParseRangeAndSort(range, "sort");


            try
            {
                var episodes = await _dBContext.Episodes
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((queryParams.Page - 1) * queryParams.PerPage)
                    .Take(queryParams.PerPage)
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
                        PublisherId = c.PublisherId,
                        CreatedDate = c.CreatedDate,
                        LastUpdate = c.LastUpdate
                    })
                    .ToListAsync();


                // Add Content-Range header
                int totalEpisodes = _dBContext.Episodes.Count();
                Response.Headers.Add("Content-Range", $"episodes {((page - 1) * perPage)}-{Math.Min(page * perPage, totalEpisodes)}/{totalEpisodes}");

                return Ok(episodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all episodes");
                return StatusCode(500, "An error occurred while processing the request. " + ex.Message);
            }
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchEpisodes(string query, int page = 1, int perPage = 10)
        {
            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 11 });
            }

            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { Message = "Search query cannot be empty.", Code = 19 });
            }

            try
            {
                var episodes = await _dBContext.Episodes
                    .Where(c => c.Title.Contains(query) || c.Tags.Contains(query) || c.Description.Contains(query))
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
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
                        PublisherId = c.PublisherId,
                        CreatedDate = c.CreatedDate,
                        LastUpdate = c.LastUpdate
                    })
                    .ToListAsync();

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"episodes {((page - 1) * perPage)}-{page * perPage}/{_dBContext.Episodes.Count()}");

                return Ok(episodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching episodes");
                return StatusCode(500, "An error occurred while processing the request. " + ex.Message);
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(long id, [FromBody] EpisodeRequest request)
        {
            try
            {
                var episode = await _dBContext.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning("Podcast with Id {PodcastId} not found", id);
                    return NotFound(new { Message = $"Episode with Id {id} not found.", Code = 59 });
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
                episode.LastUpdate = DateTime.UtcNow;

                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Episode with Id {EpisodeId} updated successfully", id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"episodes 0-1/1");

                return Ok(new { Message = "Podcast updated successfully.", Code = 89 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }


        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var episode = await _dBContext.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning("Episode with Id {EpisodeId} not found", id);
                    return NotFound(new { Message = $"Episode with Id {id} not found.", Code = 59 });
                }

                _dBContext.Episodes.Remove(episode);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation($"Episode with Id {id} deleted successfully", id);

                // Add Content-Range header

                Response.Headers.Add("Content-Range", $"episodes 0-1/1");

                return Ok(new { Message = "Podcast deleted successfully.", Code = 90 });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting episode");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
