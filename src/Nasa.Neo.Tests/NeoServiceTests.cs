using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Nasa.Neo.Api.Services;
using Nasa.Neo.Api.Settings;
using Xunit;
using Moq.Contrib.HttpClient;

namespace Nasa.Neo.Tests
{
    public class NeoServiceTests
    {
        private readonly IOptions<NeoServiceSettings> _neoServiceSettings;

        public NeoServiceTests(IOptions<NeoServiceSettings> neoServiceSettings)
        {
            _neoServiceSettings = neoServiceSettings;
        }

        [Theory]
        [InlineData(7)]
        [InlineData(6)]
        [InlineData(5)]
        [InlineData(4)]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public async Task CallNeoServiceGetTopThreeBiggestObjects(int days)
        {
            var mockHttpClientFactory = ArrangeMockClientFactory();
            var neoServices = new NeoService(mockHttpClientFactory, _neoServiceSettings);
            var startDate = new DateTime(2021, 9, 23, 0, 0, 0);
            var endDate = new DateTime(2021, 9, 23 + days, 23, 59, 59);
            
            var asteroids = (await neoServices.GetTopThreeBiggestAsteroids(startDate, endDate)).ToArray();

            asteroids.Should().HaveCountLessOrEqualTo(_neoServiceSettings.Value.MaxResults);
            asteroids.Should().OnlyContain(asteroid =>
                asteroid.CloseApproachDate >= startDate 
                && asteroid.CloseApproachDate <= endDate);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(8)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CallNeoApiWithInvalidDaysThrowsException(int days)
        {
            var mockHttpClientFactory = ArrangeMockClientFactory();
            var neoServices = new NeoService(mockHttpClientFactory, _neoServiceSettings);
            var startDate = DateTime.Now.Date.AddDays(1);
            var endDate = startDate.AddDays(days);

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await neoServices.GetTopThreeBiggestAsteroids(startDate, endDate);
            });
        }
        
        private IHttpClientFactory ArrangeMockClientFactory()
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.SetupRequest(HttpMethod.Get, r 
                    => r.RequestUri.AbsoluteUri.Contains("neo/rest/v1/feed"))
                .ReturnsResponse(GetSampleResponse());
            return handler.CreateClientFactory();;
        }

        private string GetSampleResponse()
        {
            var json = System.IO.File.ReadAllText("feed_sample_response.json");
            return json;
        }
    }
}