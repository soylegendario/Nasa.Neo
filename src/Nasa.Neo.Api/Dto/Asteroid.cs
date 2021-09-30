using System;

namespace Nasa.Neo.Api.Dto
{
    /// <summary>
    ///     Objeto de transporte para mapear en el los asteroides de respuesta de Nasa.Neo
    /// </summary>
    public class Asteroid
    {
        /// <summary>
        ///     Nombre del asteroide
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Diametro calculado del asteroide
        /// </summary>
        public float Diameter { get; set; }

        /// <summary>
        ///     Velocidad relativa
        /// </summary>
        public float RelativeVelocity { get; set; }

        /// <summary>
        ///     Fecha y hora de aproximación
        /// </summary>
        public DateTime CloseApproachDate { get; set; }

        /// <summary>
        ///     Planeta al que se aproxima
        /// </summary>
        public string Planet { get; set; }
    }
}