using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Persistencia;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Consulta
    {
        public class Ejecuta : IRequest<CarritoDto> {
            public int CarritoSesionId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, CarritoDto>
        {
            private readonly ContextoCarrito _contexto;
            //private readonly IMapper _mapper;
            private readonly ILibrosService _librosService;


            public Manejador(ContextoCarrito contexto, ILibrosService librosService)
            {
                _contexto = contexto;
                _librosService = librosService;
            }

            public async Task<CarritoDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = await _contexto.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                var carritoSesionDetalle = await _contexto.CarritoSesionDetalle.Where(x => x.CarritoSesionId == request.CarritoSesionId).ToListAsync();

                var listaProducto = new List<CarritoDetalleDto>();

                foreach (var libro in carritoSesionDetalle)
                {
                    var response = await _librosService.GetLibro(new Guid(libro.ProductoSeleccionado));
                    if (response.Resultado) {
                        var objLibro = response.Libro;
                        var carritoDetalleDto = new CarritoDetalleDto
                        {
                            TituloLibro = objLibro.Titulo,
                            FechaPublicacion = objLibro.FechaPublicacion,
                            LibroId = objLibro.LibreriaMaterialId
                        };
                        listaProducto.Add(carritoDetalleDto);
                    }
                }

                var carritoDto = new CarritoDto
                {
                    CarritoId = carritoSesion.CarritoSesionId,
                    FechaCreacionSesion = carritoSesion.FechaCreacion,
                    ListaProducto = listaProducto
                };

                return carritoDto;

            }
        }
    }
}
