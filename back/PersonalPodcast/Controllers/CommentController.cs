using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using System.Security.Claims;

namespace PersonalPodcast.Controllers
{
    [Route("comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly DBContext _dbContext;

        public CommentController(ILogger<CommentController> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new{ Message = "Invalid comment data.", Code = 54 });
            }

            var comment = new Comment
            {
                UserId = request.UserId,
                EpisodeId = request.EpisodeId,
                Date = request.Date,
                Message = request.Message,
            };

            try
            {
                _dbContext.comments.Add(comment);
                await _dbContext.SaveChangesAsync();

                var commentResponse = new CommentResponse
                {
                    Id = comment.Id,
                    UserId = comment.UserId,
                    EpisodeId = comment.EpisodeId,
                    Date = comment.Date,
                    Message = comment.Message,
                };

                _logger.LogInformation("Comment created successfully: {CommentId}", comment.Id);

                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, commentResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the comment.");
                return StatusCode(500, "An error occurred while creating the comment.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with Id {CommentId} not found", id);
                return NotFound();
            }

            var commentResponse = new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                EpisodeId = comment.EpisodeId,
                Date = comment.Date,
                Message = comment.Message,
            };

            return Ok(commentResponse);
        }

        [HttpGet("{episodeId}")]
        public async Task<IActionResult> GetCommentsByEpisodeId(long episodeId)
        {
            var comments = await _dbContext.comments
                .Where(c => c.EpisodeId == episodeId)
                .Select(c => new CommentResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    EpisodeId = c.EpisodeId,
                    Date = c.Date,
                    Message = c.Message
                })
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                _logger.LogWarning("No comments found for EpisodeId {EpisodeId}", episodeId);
                return NotFound($"No comments found for EpisodeId {episodeId}.");
            }

            return Ok(comments);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments(int page)
        {
            if (page < 1)
            {
                return BadRequest(new{ Message = "Invalid page number.", Code = 55 });
            }

            var comments = await _dbContext.comments
                .OrderByDescending(c => c.Date)
                .Skip((page - 1) * 10)
                .Take(10)
                .Select(c => new CommentResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    EpisodeId = c.EpisodeId,
                    Date = c.Date,
                    Message = c.Message
                })
                .ToListAsync();

            return Ok(comments); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new{ Message = "Invalid comment data.",Code = 56 });
            }

            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with Id {CommentId} not found", id);
                return NotFound(new { Message = $"Comment with Id {id} not found.", Code = 58 });
            }

            
            comment.UserId = request.UserId;
            comment.EpisodeId = request.EpisodeId;
            comment.Date = request.Date;
            comment.Message = request.Message;

            try
            {
                _dbContext.comments.Update(comment);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Comment with Id {CommentId} updated successfully", id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the comment.");
                return StatusCode(500, "An error occurred while updating the comment.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
          
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//TODO
            if (userId == null)
            {
                return Unauthorized();
            }

            
            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with Id {CommentId} not found", id);
                return NotFound(new { Message = $"Comment with Id {id} not found.", Code = 58 });
            }

            
            if (comment.UserId != long.Parse(userId))
            {
                _logger.LogWarning("User is not authorized to delete the comment with Id {CommentId}", id);
                return Forbid(); 
            }

            try
            {
                _dbContext.comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Comment with Id {CommentId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the comment.");
                return StatusCode(500, "An error occurred while deleting the comment.");
            }
        }
    }
}
