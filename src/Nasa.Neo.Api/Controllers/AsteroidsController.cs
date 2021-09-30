using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nasa.Neo.Api.Services;

namespace Nasa.Neo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsteroidsController : ControllerBase
    {
        private readonly INeoService _neoService;

        public AsteroidsController(INeoService neoService)
        {
            _neoService = neoService;
        }
        
        /// <summary>
        ///     Obtiene los 3 asteroides de mayor tamaño con riesgo de impacto con la Tierra en los próximos días indicados
        /// </summary>
        /// <param name="days">Días para la consulta</param>
        [HttpGet("{days:int}")]
        public async Task<IActionResult> Get(int days = 7)
        {
            if (days is <= 0 or > 7)
            {
                return BadRequest("El parámetro [días] debe ser un valor entre 1 y 7");
            }

            try
            {
                var startDate = DateTime.Now.Date.AddDays(1);
                var endDate = DateTime.Now.Date.AddDays(days);
                var asteroids = await _neoService.GetTopThreeBiggestAsteroids(startDate, endDate);
                return new OkObjectResult(asteroids);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}