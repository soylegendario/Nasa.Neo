using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Options;
using Nasa.Neo.Api.Dto;
using Nasa.Neo.Api.Settings;
using Newtonsoft.Json.Linq;

namespace Nasa.Neo.Api.Services
{
    /// <inheritdoc />
    public class NeoService : INeoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NeoServiceSettings _serviceSettings;
        private DateTime _startDate;
        private DateTime _endDate;
        
        /// <summary>
        ///     Inicializa una nueva instancia de <see cref="NeoService"/>
        /// </summary>
        /// <param name="httpClientFactory"><see cref="IHttpClientFactory"/> para llamar la servicio Nasa.Neo</param>
        /// <param name="serviceSettings">Opciones de configuración</param>
        public NeoService(IHttpClientFactory httpClientFactory,
            IOptions<NeoServiceSettings> serviceSettings)
        {
            _httpClientFactory = httpClientFactory;
            _serviceSettings = serviceSettings.Value;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Asteroid>> GetTopThreeBiggestAsteroids(DateTime startDate, DateTime endDate)
        {
            var days = (endDate - startDate).Days;
            if (days is <= 0 or > 7)
            {
                throw new Exception("La consulta al servicio debe ser de entre 1 y 7 días");
            }
            
            _startDate = startDate;
            _endDate = endDate;
            var response = await CallNeoApi();
            return response.OrderByDescending(asteroid => asteroid.Diameter)
                .Take(_serviceSettings.MaxResults);
        }

        /// <summary>
        ///     Llamada al servicio Nasa.Neo
        /// </summary>
        private async Task<IEnumerable<Asteroid>> CallNeoApi()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_serviceSettings.BaseUrl);
            var response = await client.GetAsync(Url.Combine(
                _serviceSettings.FeedMethod,
                "?",
                $"start_date={_startDate:yyyy-MM-dd}",
                $"end_date={_endDate:yyyy-MM-dd}",
                $"api_key={_serviceSettings.ApiKey}"
            ));
            response.EnsureSuccessStatusCode();
            var asteroids = await ParseResponse(response);
            return asteroids;
        }

        /// <summary>
        ///     Parseo de la respuesta obtenida del servicio Nasa.Neo
        /// </summary>
        /// <param name="response"><see cref="HttpResponseMessage"/>Mensaje de respuesta del servicio</param>
        private async Task<IEnumerable<Asteroid>> ParseResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonContent = JObject.Parse(content);
            var asteroidsForDay = jsonContent["near_earth_objects"].Children();

            return from day in asteroidsForDay
                from asteroid in day.First.Children()
                let closeApproachData = asteroid["close_approach_data"].First
                let closeApproachDate = DateTime.ParseExact(closeApproachData["close_approach_date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture)
                where asteroid["is_potentially_hazardous_asteroid"].ToObject<bool>()
                      && closeApproachDate >= _startDate && closeApproachDate <= _endDate
                let diameter = (
                    asteroid["estimated_diameter"]["kilometers"]["estimated_diameter_min"].ToObject<float>() 
                    + asteroid["estimated_diameter"]["kilometers"]["estimated_diameter_max"].ToObject<float>()
                    ) / 2
                select new Asteroid
                {
                    Name = asteroid["name"].ToString(),
                    Diameter = diameter,
                    RelativeVelocity = closeApproachData["relative_velocity"]["kilometers_per_hour"].ToObject<float>(),
                    CloseApproachDate = DateTime.Parse(closeApproachData["close_approach_date_full"].ToString()),
                    Planet = closeApproachData["orbiting_body"].ToString()
                };
        }
    }
}