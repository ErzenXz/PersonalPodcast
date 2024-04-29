using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;


namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public CategoriesController(ILogger<UserController> logger, DBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost, Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            try
            {
                var category = new Category
                {
                    Name = request.Name
                };

                _dBContext.categories.Add(category);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation($"Category created successfully: {category.Id}", category.Id);

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"categories 0-0/1");

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(long id)
        {
            try
            {
                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with Id {id} not found", id);
                    return NotFound(new { Message = $"Category with Id {id} not found.", Code = 57 });
                }

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name
                };

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"categories 0-0/1");

                return Ok(categoryResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _dBContext.categories
                    .Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToListAsync();

                // Add Content-Range header
                Response.Headers.Add("Content-Range", $"categories 0-{categories.Count() - 1}/{categories.Count()}");

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Get all podcasts that belong to a category

        [HttpGet("{id}/podcasts")]
        public async Task<IActionResult> GetCategoryPodcasts(long id, int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    _logger.LogWarning($"Invalid page number {page}", page);
                    return BadRequest(new { Message = "Invalid page number.", Code = 11 });
                }

                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with Id {id} not found", id);
                    return NotFound(new { Message = $"Category with Id {id} not found.", Code = 57 });
                }

                var podcasts = await _dBContext.Podcasts
                    .Where(p => p.CategoryId == id)
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

                return Ok(podcasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category podcasts");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpPut("{id}") ,Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(long id, [FromBody] CategoryRequest request)
        {
            try
            {
                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with Id {id} not found", id);
                    return NotFound(new { Message = $"Category with Id {id} not found.", Code = 57 });
                }

                category.Name = request.Name;

                await _dBContext.SaveChangesAsync();

                _logger.LogInformation($"Category with Id {category.Id} updated successfully", id);

                return Ok(new { Message = "Category updated successfully.", Code = 93 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with Id {id} not found", id);
                    return NotFound(new { Message = $"Category with Id {id} not found.", Code = 57 });
                }

                _dBContext.categories.Remove(category);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation($"Category with Id {category.Id} deleted successfully", id);

                return Ok(new { Message = " Category deleted successfully.", Code = 94 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
