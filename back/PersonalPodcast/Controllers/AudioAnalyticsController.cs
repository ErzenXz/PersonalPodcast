using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
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

                return CreatedAtAction(nameof(GetAudioAnalytics), new { id = audioAnalytics.Id }, audioAnalyticsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the audio analytics.");
                return StatusCode(500, "An error occurred while creating the audio analytics.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudioAnalytics(long id)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound();
                }

                var audioAnalyticsResponse = new AudioAnalyticsResponse
                {
                    UserId = audioAnalytics.UserId,
                    EpisodeId = audioAnalytics.EpisodeId,
                    FirstPlay = audioAnalytics.FirstPlay,
                    LastPlay = audioAnalytics.LastPlay,
                    Length = audioAnalytics.Length
                };

                return Ok(audioAnalyticsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the audio analytics.");
                return StatusCode(500, "An error occurred while retrieving the audio analytics.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAudioAnalytics()
        {
            try
            {
                var audioAnalyticsList = await _dBContext.audioAnalytics.Select(a => new AudioAnalyticsResponse
                {
                    UserId = a.UserId,
                    EpisodeId = a.EpisodeId,
                    FirstPlay = a.FirstPlay,
                    LastPlay = a.LastPlay,
                    Length = a.Length
                }).ToListAsync();

                return Ok(audioAnalyticsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all audio analytics.");
                return StatusCode(500, "An error occurred while retrieving all audio analytics.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AudioAnalyticsRequest request)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics.FindAsync(id);
                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound($"Audio analytics with Id {id} not found.");
                }

                audioAnalytics.UserId = request.UserId;
                audioAnalytics.EpisodeId = request.EpisodeId;
                audioAnalytics.FirstPlay = request.FirstPlay;
                audioAnalytics.LastPlay = request.LastPlay;
                audioAnalytics.Length = request.Length;

                _dBContext.audioAnalytics.Update(audioAnalytics);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Audio analytics with Id {AudioAnalyticsId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the audio analytics.");
                return StatusCode(500, "An error occurred while updating the audio analytics.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var audioAnalytics = await _dBContext.audioAnalytics.FindAsync(id);
                if (audioAnalytics == null)
                {
                    _logger.LogWarning("Audio analytics with Id {AudioAnalyticsId} not found", id);
                    return NotFound($"Audio analytics with Id {id} not found.");
                }

                _dBContext.audioAnalytics.Remove(audioAnalytics);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Audio analytics with Id {AudioAnalyticsId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the audio analytics.");
                return StatusCode(500, "An error occurred while deleting the audio analytics.");
            }
        }
    }
}
