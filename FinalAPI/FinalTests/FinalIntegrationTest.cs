using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Threading.Tasks;
using FinalAPI.Controllers;

namespace FinalAPI.Tests
{
    public class FinalIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public FinalIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users/list");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}