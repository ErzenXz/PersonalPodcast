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
    [Route("analytics")]
    public class AudioAnalyticsController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public AudioAnalyticsController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost, Authorize(Roles ="User,Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] AudioAnalyticsRequest request)
        {
            try
            {
                var audioAnalytics = new AudioAnalytics
                {
                    UserId= request.UserId,
                    EpisodeId= request.EpisodeId,
                    FirstPlay=  request.FirstPlay,
                    LastPlay= request.LastPlay,
                    Length= request.Length,

                };

                if (audioAnalytics.FirstPlay > audioAnalytics.LastPlay)
                {
                    _logger.LogWarning("First play date cannot be greater than last play date.");
                    return BadRequest(new {Message= "First play date cannot be greater than last play date.", Code =20});
                }

                if (audioAnalytics.LastPlay > audioAnalytics.FirstPlay.AddSeconds(audioAnalytics.Length))
                {
                    _logger.LogWarning("Last play date cannot be greater than the length of the audio.");
                    return BadRequest(new {Message= "Last play date cannot be greater than the length of the audio." , Code = 21});
                }

                if (audioAnalytics.Length <= 0)
                {
                    _logger.LogWarning("Length of the audio must be greater than 0.");
                    return BadRequest(new {Message= "Length of the audio must be greater than 0." ,Code=22});
                }

                // If the audio analytics already exists, update the existing record
                var existingAudioAnalytics = await _dBContext.audioAnalytics.FirstOrDefaultAsync(a => a.UserId == audioAnalytics.UserId && a.EpisodeId == audioAnalytics.EpisodeId);
                if (existingAudioAnalytics != null)
                {
                    existingAudioAnalytics.FirstPlay = audioAnalytics.FirstPlay;
                    existingAudioAnalytics.LastPlay = audioAnalytics.LastPlay;
                    existingAudioAnalytics.Length = audioAnalytics.Length;

                    _dBContext.audioAnalytics.Update(existingAudioAnalytics);
                    await _dBContext.SaveChangesAsync();

                    _logger.LogInformation("Audio analytics updated successfully: {AudioAnalyticsId}", existingAudioAnalytics.Id);

                    return Ok(new { Message = "Audio Analytics updated successfully.", Code = 95 });
                }

                _dBContext.audioAnalytics.Add(audioAnalytics);
                await _dBContext.SaveChangesAsync();

                var audioAnalyticsResponse = new AudioAnalyticsResponse
                {
                    UserId = audioAnalytics.UserId,
                    EpisodeId = audioAnalytics.EpisodeId,
                    FirstPlay = audioAnalytics.FirstPlay,
                    LastPlay = audioAnalytics.LastPlay,
                    Length = audioAnalytics.Length
                };

                _logger.LogInformation("Audio analytics created successfully: {AudioAnalyticsId}", audioAnalytics.Id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"audioAnalytics 0-0/1");

                return CreatedAtAction(nameof(GetAudioAnalytics), new { id = audioAnalytics.Id }, audioAnalyticsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the audio analytics.");
                return StatusCode(500, "An error occurred while creating the audio analytics.");
            }
        }

        [HttpGet("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAudioAnalytics(long id)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound(new {Message= $"Audio analytics with Id {id} not found", Code= 64 });
                }

                var audioAnalyticsResponse = new AudioAnalyticsResponse
                {
                    UserId = audioAnalytics.UserId,
                    EpisodeId = audioAnalytics.EpisodeId,
                    FirstPlay = audioAnalytics.FirstPlay,
                    LastPlay = audioAnalytics.LastPlay,
                    Length = audioAnalytics.Length
                };

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"audioAnalytics 0-0/1");

                return Ok(audioAnalyticsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the audio analytics.");
                return StatusCode(500, "An error occurred while retrieving the audio analytics.");
            }
        }

        [HttpGet, Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllAudioAnalytics(int page = 1)
        {
            try
            {

                if (page < 0)
                {
                    _logger.LogWarning("Page number cannot be less than 0.");
                    return BadRequest(new { Message = "Invalid page number.", Code = 11 });
                }

                var audioAnalyticsList = await _dBContext.audioAnalytics.Skip(page * 10).Take(10).
                    Select(a => new AudioAnalyticsResponse
                {
                    UserId = a.UserId,
                    EpisodeId = a.EpisodeId,
                    FirstPlay = a.FirstPlay,
                    LastPlay = a.LastPlay,
                    Length = a.Length
                }).ToListAsync();

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"audioAnalytics 0-{audioAnalyticsList.Count() - 1}/{audioAnalyticsList.Count()}");

                return Ok(audioAnalyticsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all audio analytics.");
                return StatusCode(500, "An error occurred while retrieving all audio analytics.");
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(long id, [FromBody] AudioAnalyticsRequest request)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics.FindAsync(id);
                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound(new { Message = $"Audio analytics with Id {id} not found", Code = 64 });
                }

                audioAnalytics.UserId = request.UserId;
                audioAnalytics.EpisodeId = request.EpisodeId;
                audioAnalytics.FirstPlay = request.FirstPlay;
                audioAnalytics.LastPlay = request.LastPlay;
                audioAnalytics.Length = request.Length;

                _dBContext.audioAnalytics.Update(audioAnalytics);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Audio analytics with Id {AudioAnalyticsId} updated successfully", id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"audioAnalytics 0-0/1");

                return Ok(new { Message = "Audio Analytics updated successfully.", Code = 95 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the audio analytics.");
                return StatusCode(500, "An error occurred while updating the audio analytics.");
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics.FindAsync(id);
                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound(new { Message = $"Audio analytics with Id {id} not found", Code = 64 });
                }

                _dBContext.audioAnalytics.Remove(audioAnalytics);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Audio analytics with Id {AudioAnalyticsId} deleted successfully", id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"audioAnalytics 0-0/1");

                return Ok(new { Message = " Audio Analytics deleted successfully.", Code = 96 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the audio analytics.");
                return StatusCode(500, "An error occurred while deleting the audio analytics.");
            }
        }

        // Get recomented episodes based on some user watch time and tags in a episode

        [HttpGet("recommendedEpisodes")]
        public async Task<IActionResult> GetRecommendedEpisodes(int page = 1)
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];


                if (refreshToken == null)
                {
                    // Return a paginated list of random episodes
                    var randomEpisodes = await _dBContext.Episodes
                        .OrderBy(e => Guid.NewGuid()) // Shuffle the episodes
                        .Skip((page - 1) * 10)
                        .Take(10)
                        .ToListAsync();

                    // Add Content-Range header
                    Response.Headers.Add("Content-Range", $"episodes 0-{randomEpisodes.Count - 1}/{randomEpisodes.Count}");

                    return Ok(randomEpisodes);
                } else
                {
                    var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                    var userAudioAnalytics = await _dBContext.audioAnalytics
                    .Where(a => a.UserId == user.Id)
                    .ToListAsync();

                if (!userAudioAnalytics.Any())
                {
                   // Return a paginated list of random episodes
                        var randomEpisodes = await _dBContext.Episodes
                            .OrderBy(e => Guid.NewGuid()) // Shuffle the episodes
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync();

                        _logger.LogWarning("No audio analytics found for user with Id {UserId}", user.Id);

                        // Add Content-Range header
                        Response.Headers.Add("Content-Range", $"episodes 0-{randomEpisodes.Count - 1}/{randomEpisodes.Count}");
                        return Ok(randomEpisodes);
                    //return NotFound(new { Message = $"No audio analytics found for user with Id {user.Id}", Code = 65 });
                }

                // Calculate average watch time per tag
                var tagWatchTimes = userAudioAnalytics
                    .GroupBy(a => a.Episode.Tags)
                    .Select(g => new { Tag = g.Key, AverageWatchTime = g.Average(a => a.Length) })
                    .OrderByDescending(t => t.AverageWatchTime)
                    .ToList();

                if (!tagWatchTimes.Any())
                {

                        // Return a paginated list of random episodes
                        var randomEpisodes = await _dBContext.Episodes
                            .OrderBy(e => Guid.NewGuid()) // Shuffle the episodes
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync();

                        _logger.LogWarning("No tags with watch time found for user with Id {UserId}", user.Id);

                        // Add Content-Range header
                        Response.Headers.Add("Content-Range", $"episodes 0-{randomEpisodes.Count - 1}/{randomEpisodes.Count}");
                        return Ok(randomEpisodes);

                    //return NotFound(new { Message = $"No tags with watch time found for user with Id {user.Id}", Code = 66 });
                }

                // Get top tags based on watch time
                var topTags = tagWatchTimes.Take(5).Select(t => t.Tag).ToList();

                // Recommend episodes with similar tags
                var recommendedEpisodes = await _dBContext.Episodes
                    .Where(e => e.Tags != null && e.Tags.Split(new char[] { ',' }, StringSplitOptions.None).Any(tag => topTags.Contains(tag)))
                    .ToListAsync();

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"episodes 0-{recommendedEpisodes.Count - 1}/{recommendedEpisodes.Count}");

                return Ok(recommendedEpisodes);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the recommended episodes.");
                return StatusCode(500, "An error occurred while retrieving the recommended episodes.");
            }
        }

    }
}
