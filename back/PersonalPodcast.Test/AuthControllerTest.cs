using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using PersonalPodcast.Controllers;
using PersonalPodcast.Data;
using PersonalPodcast.Models;
using System.Net;

namespace PersonalPodcast.Test
{
    public class AuthControllerTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private Mock<DBContext> _dbContextMock = new (null);


        public AuthControllerTest()
        {
            _configuration = new Mock<IConfiguration>();

            _configuration.SetupGet(x => x[It.Is<string>(s => s == "AppSecurity:Secret")]).Returns("testkeyklasklfjsaklfsa sfksafkljfsaklsafj sfalkjsfakljsfaklsafj lkafsjklasfkjlfsajklfs");
        }


        [Fact]
        public void RegisterRequestWithInvalidPassword_ReturnsBadRequestWithCode3()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
                .Options;

            var dbContext = new DBContext(options);

            var registerRequest = new UserRegisterRequest
            {
                Username = "test77",
                FullName = "Test",
                Email = "test77@test.com",
                Password = "123456",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "",
                Birthdate = DateTime.Now,
                Role = "User"
            };

            // Act
            var controller = new AuthController(_configuration.Object, dbContext);

            var result = controller.Register(registerRequest).Result as BadRequestObjectResult;


            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            // Use reflection to get the 'Code' property value
            var badRequestValue = result.Value.GetType().GetProperty("Code").GetValue(result.Value, null);
            Assert.Equal(3, badRequestValue);




        }

        [Fact]
        public void RegisterRequestWithUsedEmail_ReturnsBadRequestWithCode102()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
                .Options;

            var dbContext = new DBContext(options);

            // Start a transaction
            dbContext.Database.BeginTransaction();

            var registerRequest = new UserRegisterRequest
            {
                Username = "test424",
                FullName = "Test",
                Email = "test@test.com",
                Password = "testtest12345",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "",
                Birthdate = DateTime.Now,
                Role = "User"
            };

            // Act
            var controller = new AuthController(_configuration.Object, dbContext);

            var result = controller.Register(registerRequest).Result as BadRequestObjectResult;


            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            // Use reflection to get the 'Code' property value
            var badRequestValue = result.Value.GetType().GetProperty("Code").GetValue(result.Value, null);
            Assert.Equal(102, badRequestValue);

            // Cleanup
            dbContext.Database.RollbackTransaction();
            dbContext.Dispose();

        }

        [Fact]
        public void RegisterRequestWithUsedUsername_ReturnsBadRequestWithCode103()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseMySql("Server=localhost;Database=personal_podcast;User=root;Password=;", ServerVersion.AutoDetect("Server=localhost;Database=personal_podcast;User=root;Password=;"))
                .Options;

            var dbContext = new DBContext(options);

            // Start a transaction
            dbContext.Database.BeginTransaction();

            var registerRequest = new UserRegisterRequest
            {
                Username = "test",
                FullName = "Test",
                Email = "test9999@test.com",
                Password = "testtest12345",
                FirstLogin = DateTime.Now,
                LastLogin = DateTime.Now,
                ConnectingIp = "",
                Birthdate = DateTime.Now,
                Role = "User"
            };

            // Act
            var controller = new AuthController(_configuration.Object, dbContext);

            var result = controller.Register(registerRequest).Result as BadRequestObjectResult;


            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            // Use reflection to get the 'Code' property value
            var badRequestValue = result.Value.GetType().GetProperty("Code").GetValue(result.Value, null);
            Assert.Equal(104, badRequestValue);

            dbContext.Database.RollbackTransaction();
            dbContext.Dispose();
        }

    }
}