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
}