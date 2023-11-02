using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System.Net;
using FinalAPI;

namespace FinalAPI.Tests
{
    public class FinalUnitTests
    {
        private readonly UserService _userService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public FinalUnitTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            _userService = new UserService(new HttpClient(_mockHttpMessageHandler.Object));
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnCorrectUsers()
        {
            // Arrange
            var mockData = new UserData
            {
                data = new[]
                {
                    new UserResponse { Nickname = "TestUser", UserID = "1", FirstSeen = "2023-11-01" }
                }
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(mockData))
                });

            // Act
            var result = (await _userService.GetUsersAsync()).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestUser", result[0].Nickname);
        }
    }
}
