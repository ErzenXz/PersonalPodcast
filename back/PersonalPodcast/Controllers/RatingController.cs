using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Authorization;
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
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] RatingRequest request)
        {
            try
            {

                if (request == null)
                {
                    return BadRequest(new { Message = "Invalid rating data.", Code = 13 });
                }

                if (request.RatingValue < 1 || request.RatingValue > 5)
                {
                    return BadRequest(new { Message = "Rating value must be between 1 and 5.", Code = 14 });
                }

                if(await _dBContext.ratings.AnyAsync(r => r.UserId == request.UserId && r.EpisodeId == request.EpisodeId))
                {
                    // Update the existing rating
                    var existingRating = await _dBContext.ratings
                        .Where(r => r.UserId == request.UserId && r.EpisodeId == request.EpisodeId)
                        .FirstOrDefaultAsync();
                    if (existingRating != null)
                    {
                           existingRating.RatingValue = request.RatingValue;
                        existingRating.Date = request.Date;
                        _dBContext.ratings.Update(existingRating);
                        await _dBContext.SaveChangesAsync();

                        var ratingResponse = new RatingResponse
                        {
                            Id = existingRating.Id,
                            UserId = existingRating.UserId,
                            EpisodeId = existingRating.EpisodeId,
                            RatingValue = existingRating.RatingValue,
                            Date = existingRating.Date
                        };

                        _logger.LogInformation("Rating updated successfully: {RatingId}", existingRating.Id);

                        return CreatedAtAction(nameof(GetRating), new { id = existingRating.Id }, ratingResponse);  
                    }

                    return NotFound();
                } else
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

                var ratingResponse2 = new RatingResponse
                {
                    Id = rating.Id,
                    UserId = rating.UserId,
                    EpisodeId = rating.EpisodeId,
                    RatingValue = rating.RatingValue,
                    Date = rating.Date
                };

                _logger.LogInformation("Rating created successfully: {RatingId}", rating.Id);

                 // Add Content-Range header
                 Response.Headers.Add("Content-Range", $"ratings 0-0/1");

                return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, ratingResponse2);
                
                }

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
                return NotFound(new { Message = $"Rating with Id {id} not found.", Code = 62 });
            }

            var ratingResponse = new RatingResponse
            {
                Id = rating.Id,
                UserId = rating.UserId,
                EpisodeId = rating.EpisodeId,
                RatingValue = rating.RatingValue,
                Date = rating.Date
            };

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"ratings 0-0/1");

            return Ok(ratingResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings(int page = 1)
        {

            if (page < 1)
            {
                return BadRequest(new { Message = "Invalid page number.", Code = 11 });
            }

            var ratings = await _dBContext.ratings.Skip((page - 1) * 10).Take(10).Select(r => new RatingResponse                
            {
                Id = r.Id,
                UserId = r.UserId,
                EpisodeId = r.EpisodeId,
                RatingValue = r.RatingValue,
                Date = r.Date
            }).ToListAsync();

            // Add Content-Range header
            Response.Headers.Add("Content-Range", $"ratings 0-{ratings.Count() - 1}/{ratings.Count()}");

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
                    return Ok(-1);
                }

                double averageRating = episodeRatings.Average();

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"ratings 0-0/1");
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating the average rating for the episode.");
                return StatusCode(500, "An error occurred while calculating the average rating for the episode.");
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(long id, [FromBody] RatingRequest request)
        {

            try
            {
                var rating = await _dBContext.ratings.FindAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with Id {RatingId} not found", id);
                    return NotFound(new { Message = $"Rating with Id {id} not found.", Code = 62 });
                }

                if (request.RatingValue < 1 || request.RatingValue > 5)
                {
                    return BadRequest(new { Message = "Rating value must be between 1 and 5.", Code = 14 });
                }


                rating.UserId = request.UserId;
                rating.EpisodeId = request.EpisodeId;
                rating.RatingValue = request.RatingValue;
                rating.Date = request.Date;

                _dBContext.ratings.Update(rating);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Rating with Id {RatingId} updated successfully", id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"ratings 0-0/1");

                return Ok(new { Message = "Rating updated successfully.", Code = 85 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rating.");
                return StatusCode(500, "An error occurred while updating the rating.");
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(long id)
        {

            try
            {
                var rating = await _dBContext.ratings.FindAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning("Rating with Id {RatingId} not found", id);
                    return NotFound(new { Message = $"Rating with Id {id} not found.", Code = 62 });
                }

                _dBContext.ratings.Remove(rating);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Rating with Id {RatingId} deleted successfully", id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"ratings 0-0/1");

                return Ok(new { Message = "Rating deleted successfully.", Code = 86 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rating.");
                return StatusCode(500, "An error occurred while deleting the rating.");
            }
        }

    }
}
