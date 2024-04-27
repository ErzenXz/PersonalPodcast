﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using System;
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

        [HttpPost, Authorize(Roles = "User,Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CommentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new{ Message = "Invalid comment data.", Code = 18 });
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
        public async Task<IActionResult> GetComment(long id)
        {
            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning($"Comment with Id {id} not found", id);
                return NotFound(new { Message = $"Comment with Id {id} not found.", Code = 58 });
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

        [HttpGet("episodes/{episodeId}")]
        public async Task<IActionResult> GetCommentsByEpisodeId(int episodeId, int page = 1)
        {
            var comments = await _dbContext.comments
                .Where(c => c.EpisodeId == episodeId)
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

            if (comments == null || !comments.Any())
            {
                _logger.LogWarning("No comments found for EpisodeId {EpisodeId}", episodeId);
                return NotFound(new {Message = $"No comments found for EpisodeId {episodeId}.", Code = 63});
            }

            return Ok(comments);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments(int page = 1)
        {
            if (page < 1)
            {
                return BadRequest(new{ Message = "Invalid page number.", Code = 11 });
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

        [HttpPut("{id}"), Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new{ Message = "Invalid comment data.",Code = 17 });
            }

            DateTime maxAllowedTimeBefore = DateTime.UtcNow.AddMinutes(-10);

            if (request.Date > DateTime.UtcNow || request.Date < maxAllowedTimeBefore)
            {
                return BadRequest(new { Message = "Invalid date.", Code = 10 });
            }

            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning($"Comment with Id {id} not found", id);
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
                return Ok(new { Message = "Comment updated successfully.", Code = 91 }); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the comment.");
                return StatusCode(500, "An error occurred while updating the comment.");
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(long id)
        {
                     
            var comment = await _dbContext.comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning($"Comment with Id {id} not found", id);
                return NotFound(new { Message = $"Comment with Id {id} not found.", Code = 58 });
            }

            try
            {
                _dbContext.comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Comment with Id {id} deleted successfully", id);
                return Ok(new { Message = " Comment deleted successfully.", Code = 92 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the comment with ID {id}.");
                return StatusCode(500, "An error occurred while deleting the comment.");
            }
        }
    }
}
