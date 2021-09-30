using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nasa.Neo.Api.Services;
using Nasa.Neo.Api.Settings;

namespace Nasa.Neo.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var neoServiceSettings = new NeoServiceSettings
            {
                BaseUrl = "https://api.nasa.gov/",
                FeedMethod = "neo/rest/v1/feed",
                ApiKey = "zdUP8ElJv1cehFM0rsZVSQN7uBVxlDnu4diHlLSb",
                MaxResults = 3
            };
            services.AddSingleton<IOptions<NeoServiceSettings>>(neoServiceSettings);

            services.AddHttpClient();
        }    
    }
}