using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nasa.Neo.Api.Dto;

namespace Nasa.Neo.Api.Services
{
    /// <summary>
    ///     Servicio para implementar la petición de datos a Nasa.Neo
    /// </summary>
    public interface INeoService
    {
        /// <summary>
        ///     Obtiene los tres asteroides mayores con riesgo de impacto con la Tierra en las fechas dadas
        /// </summary>
        /// <param name="startDate">Fecha inicial</param>
        /// <param name="endDate">Fecha final</param>
        /// <exception cref="Exception">
        ///     Excepción si entre la fecha incial y final hay más de 7 días ya que la Nasa no permite la llamada.
        /// </exception>
        Task<IEnumerable<Asteroid>> GetTopThreeBiggestAsteroids(DateTime startDate, DateTime endDate);
    }
}