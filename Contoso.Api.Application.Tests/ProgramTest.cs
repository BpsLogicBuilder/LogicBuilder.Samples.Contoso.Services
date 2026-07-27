using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.Api.Application.Tests
{
    public class ProgramTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Get_WeatherForecast_ReturnsSuccessAndData()
        {
            // Act: Send an HTTP request to the running application
            var response = await _client.GetAsync("/weatherforecast", CancellationToken.None);

            // Assert: Verify the response properties
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync(CancellationToken.None);
            Assert.NotEmpty(content);
        }
    }
}
