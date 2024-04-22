using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using System.Security.Claims;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("ratings")]
    public class RatingsController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public RatingsController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RatingRequest request)
        {
            try
            {
                var rating = new Rating
                {
                    UserId = request.UserId,
                    EpisodeId = request.EpisodeId,
                    RatingValue = request.RatingValue,
                    Date = request.Date
                };

                _dBContext.ratings.Add(rating);
                await _dBContext.SaveChangesAsync();

                var ratingResponse = new RatingResponse
                {
                    Id = rating.Id,
                    UserId = rating.UserId,
                    EpisodeId = rating.EpisodeId,
                    RatingValue = rating.RatingValue,
                    Date = rating.Date
                };

                _logger.LogInformation("Rating created successfully: {RatingId}", rating.Id);

                return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, ratingResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the rating.");
                return StatusCode(500, "An error occurred while creating the rating.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRating(long id)
        {
            var rating = await _dBContext.ratings.FindAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Rating with Id {RatingId} not found", id);
                return NotFound();
            }

            var ratingResponse = new RatingResponse
            {
                Id = rating.Id,
                UserId = rating.UserId,
                EpisodeId = rating.EpisodeId,
                RatingValue = rating.RatingValue,
                Date = rating.Date
            };

            return Ok(ratingResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _dBContext.ratings.Select(r => new RatingResponse
            {
                Id = r.Id,
                UserId = r.UserId,
                EpisodeId = r.EpisodeId,
                RatingValue = r.RatingValue,
                Date = r.Date
            }).ToListAsync();

            return Ok(ratings);
        }

        [HttpGet("episode/{episodeId}/average")]
        public async Task<IActionResult> GetEpisodeAverageRating(long episodeId)
        {
            try
            {
                var episodeRatings = await _dBContext.ratings
                    .Where(r => r.EpisodeId == episodeId)
                    .Select(r => r.RatingValue)
                    .ToListAsync();

                if (episodeRatings.Count == 0)
                {
                    return Ok(0);
                }

                double averageRating = episodeRatings.Average();
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating the average rating for the episode.");
                return StatusCode(500, "An error occurred while calculating the average rating for the episode.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] RatingRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var rating = await _dBContext.ratings.FindAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with Id {RatingId} not found", id);
                    return NotFound($"Rating with Id {id} not found.");
                }

                if (rating.UserId != long.Parse(userId))
                {
                    return Forbid();
                }

                rating.UserId = request.UserId;
                rating.EpisodeId = request.EpisodeId;
                rating.RatingValue = request.RatingValue;
                rating.Date = request.Date;

                _dBContext.ratings.Update(rating);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Rating with Id {RatingId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rating.");
                return StatusCode(500, "An error occurred while updating the rating.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var rating = await _dBContext.ratings.FindAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with Id {RatingId} not found", id);
                    return NotFound($"Rating with Id {id} not found.");
                }

                if (rating.UserId != long.Parse(userId))
                {
                    return Forbid();
                }

                _dBContext.ratings.Remove(rating);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Rating with Id {RatingId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rating.");
                return StatusCode(500, "An error occurred while deleting the rating.");
            }
        }

    }
}
