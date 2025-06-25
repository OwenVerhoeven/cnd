using cnd.Controllers;
using cnd.Models.Auth;
using cnd.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace JournalApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkWithToken()
        {
            var request = new RegisterRequest { Username = "user1", Password = "pass" };
            _mockAuthService.Setup(x => x.RegisterAsync("user1", "pass"))
                .ReturnsAsync("mocked-token");

            var result = await _controller.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenObj = okResult.Value.GetType().GetProperty("token")?.GetValue(okResult.Value);
            Assert.Equal("mocked-token", tokenObj);
        }

        [Fact]
        public async Task Register_ReturnsBadRequestIfTokenIsNull()
        {
            var request = new RegisterRequest { Username = "user2", Password = "pass" };
            _mockAuthService.Setup(x => x.RegisterAsync("user2", "pass"))
                .ReturnsAsync((string?)null);

            var result = await _controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOkWithToken()
        {
            var request = new LoginRequest { Username = "user1", Password = "pass" };
            _mockAuthService.Setup(x => x.LoginAsync("user1", "pass"))
                .ReturnsAsync("jwt-token");

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenObj = okResult.Value.GetType().GetProperty("token")?.GetValue(okResult.Value);
            Assert.Equal("jwt-token", tokenObj);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorizedIfTokenIsNull()
        {
            var request = new LoginRequest { Username = "userX", Password = "fail" };
            _mockAuthService.Setup(x => x.LoginAsync("userX", "fail"))
                .ReturnsAsync((string?)null);

            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
