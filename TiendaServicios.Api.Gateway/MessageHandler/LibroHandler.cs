using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Gateway.InterfaceRemote;
using TiendaServicios.Api.Gateway.LibroRemote;

namespace TiendaServicios.Api.Gateway.MessageHandler
{
    public class LibroHandler: DelegatingHandler
    {
        private readonly ILogger<LibroHandler> _logger;
        private readonly IAutorRemote _autorRemote;
        public LibroHandler(ILogger<LibroHandler> logger, IAutorRemote autorRemote) {
            _logger = logger;
            _autorRemote = autorRemote;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) {

            var tiempo = Stopwatch.StartNew();
            _logger.LogInformation("start request");
            var response = await base.SendAsync(request, token);
            if (response.IsSuccessStatusCode) {
                var contenido = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<LibroModeloRemote>(contenido, options);
                var responseAutor = await _autorRemote.GetAutor(resultado.AutorLibro ?? Guid.Empty);
                if (responseAutor.result) {
                    var objAutor = responseAutor.autor;
                    resultado.AutorData = objAutor;
                    var resultadoStr = JsonSerializer.Serialize(resultado);
                    response.Content = new StringContent(resultadoStr, System.Text.Encoding.UTF8, "application/json");

                }

            }
            _logger.LogInformation($"end request {tiempo.ElapsedMilliseconds}ms");
            return response;
        }
    }
}
