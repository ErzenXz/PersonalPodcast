using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalPodcast.Controllers;
using PersonalPodcast.Data;
using PersonalPodcast.DTO;
using PersonalPodcast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CategoriesControllerTest
{
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly Mock<DBContext> _dbContextMock;

    public CategoriesControllerTest()
    {
        _loggerMock = new Mock<ILogger<UserController>>();
        _dbContextMock = new Mock<DBContext>();
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtAction()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<DBContext>()
               .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
               .Options;
        var dbContext = new DBContext(options);

        var categoryRequest = new CategoryRequest
        {
            Name = "TestCategory"
        };

        //Act
        var controller = new CategoriesController(_loggerMock.Object, dbContext);

        var result = controller.Create(categoryRequest).Result as CreatedAtActionResult;

        var id = result.Value.GetType().GetProperty("Id").GetValue(result.Value, null);

        if (id != null)
        {
            var result2 = controller.GetCategory((long)id).Result;
        }

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("GetCategory", result.ActionName);
        Assert.Single(result.RouteValues);
        Assert.IsType<CreatedAtActionResult>(result);
    }
    
    [Fact]
    public async Task GetCategory_ReturnsOkResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
            .Options;
        var dbContext = new DBContext(options);

        var category = new Category
        {
            Name = "TestCategory"
        };

        await dbContext.categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var controller = new CategoriesController(_loggerMock.Object, dbContext);

        // Act
        var result = controller.GetCategory(category.Id).Result;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var categoryResponse = result as OkObjectResult;
        Assert.NotNull(categoryResponse.Value);
        Assert.Equal(category.Id, (categoryResponse.Value as CategoryResponse).Id);
        Assert.Equal(category.Name, (categoryResponse.Value as CategoryResponse).Name);
    }


    [Fact]
    public async Task Update_ReturnsNoContentResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
            .Options;
        var dbContext = new DBContext(options);

        var category = new Category
        {
            Name = "TestCategory"
        };

        await dbContext.categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var controller = new CategoriesController(_loggerMock.Object, dbContext);

        var updatedRequest = new CategoryRequest
        {
            Name = "UpdatedTestCategory"
        };

        // Act
        var result = await controller.Update(category.Id, updatedRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        // Verify the update
        var updatedCategory = await dbContext.categories.FindAsync(category.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal(updatedRequest.Name, updatedCategory.Name);
    }
    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
            .Options;
        var dbContext = new DBContext(options);

        var category = new Category
        {
            Name = "TestCategory"
        };

        await dbContext.categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var controller = new CategoriesController(_loggerMock.Object, dbContext);

        // Act
        var result = await controller.Delete(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        // Verify the deletion
        var deletedCategory = await dbContext.categories.FindAsync(category.Id);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsOkResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
            .Options;
        var dbContext = new DBContext(options);

        var category1 = new Category
        {
            Name = "TestCategory1"
        };

        var category2 = new Category
        {
            Name = "TestCategory2"
        };

        await dbContext.categories.AddRangeAsync(category1, category2);
        await dbContext.SaveChangesAsync();

        var controller = new CategoriesController(_loggerMock.Object, dbContext);

        // Act
        var result = controller.GetAllCategories().Result;

        // Assert
        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var categories = result as OkObjectResult;
        Assert.NotNull(categories.Value);

        // Cast the value to the appropriate type
        var categoryList = categories.Value as List<CategoryResponse>;


        Assert.Contains(categoryList, c => c.Id == category1.Id && c.Name == category1.Name);
        Assert.Contains(categoryList, c => c.Id == category2.Id && c.Name == category2.Name);
    }
}
