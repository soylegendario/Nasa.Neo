using Microsoft.Extensions.Options;
using Nasa.Neo.Api.Services;

namespace Nasa.Neo.Api.Settings
{
    /// <summary>
    ///     Opciones de configuración de <see cref="NeoService"/>
    /// </summary>
    public class NeoServiceSettings : IOptions<NeoServiceSettings>
    {
        /// <summary>
        ///     URL base del servicio de la nasa
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        ///     Método para obtener los asteroides entre fechas
        /// </summary>
        public string FeedMethod { get; set; }
        
        /// <summary>
        ///     Máximo de asteroides devuelto
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        ///  Api key para llamar al servicio Nasa.Neo
        /// </summary>
        public string ApiKey { get; set; }

        /// <inheritdoc />
        public NeoServiceSettings Value => this;
    }
}