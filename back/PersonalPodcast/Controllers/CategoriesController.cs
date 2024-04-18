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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            try
            {
                var category = new Category
                {
                    Name = request.Name,
                    Podcast = request.Podcast
                };

                _dBContext.categories.Add(category);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Category created successfully: {CategoryId}", category.Id);

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
                    _logger.LogWarning("Category with Id {CategoryId} not found", id);
                    return NotFound($"Category with Id {id} not found.");
                }

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Podcast = category.Podcast
                };

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
                        Name = c.Name,
                        Podcast = c.Podcast
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CategoryRequest request)
        {
            try
            {
                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with Id {CategoryId} not found", id);
                    return NotFound($"Category with Id {id} not found.");
                }

                category.Name = request.Name;
                category.Podcast = request.Podcast;

                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Category with Id {CategoryId} updated successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var category = await _dBContext.categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with Id {CategoryId} not found", id);
                    return NotFound($"Category with Id {id} not found.");
                }

                _dBContext.categories.Remove(category);
                await _dBContext.SaveChangesAsync();

                _logger.LogInformation("Category with Id {CategoryId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
