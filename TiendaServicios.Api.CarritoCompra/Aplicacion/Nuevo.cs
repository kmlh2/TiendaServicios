using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Modelo;
using TiendaServicios.Api.CarritoCompra.Persistencia;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public DateTime FechaCreacion { get; set; }
            public List<string> ProductoLista { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.FechaCreacion).NotEmpty();
                RuleFor(x => x.ProductoLista).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoCarrito _contexto;

            public Manejador(ContextoCarrito contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = new CarritoSesion
                {
                    FechaCreacion = request.FechaCreacion,
                };

                _contexto.CarritoSesion.Add(carritoSesion);
                var valor = await _contexto.SaveChangesAsync();
                if (valor == 0) {
                    throw new Exception("No se pudo crear la sesion del carrito");
                }
                int id = carritoSesion.CarritoSesionId;

                foreach (var obj in request.ProductoLista) {
                    var detalleSesion = new CarritoSesionDetalle
                    {
                        FechaCreacion = DateTime.Now,
                        ProductoSeleccionado = obj,
                        CarritoSesionId = id
                    };
                    _contexto.CarritoSesionDetalle.Add(detalleSesion);
                }
                valor = await _contexto.SaveChangesAsync();
                if (valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo insertar el detalle del carrito");
            }
        }
    }
}
