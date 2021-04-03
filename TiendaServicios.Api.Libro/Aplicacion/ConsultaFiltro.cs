using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class ConsultaFiltro
    {
        public class Ejecuta : IRequest<LibroMaterialDto>
        {
            public Guid LibroGuid { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, LibroMaterialDto> 
        {
            public readonly ContextoLibreria _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoLibreria contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }

            public async Task<LibroMaterialDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = await _contexto.LibreriaMaterial.Where(x => x.LibreriaMaterialId == request.LibroGuid).FirstOrDefaultAsync();

                if (libro == null)
                {
                    throw new Exception("No se encontro el libro");
                }

                var libroDto = _mapper.Map<LibreriaMaterial, LibroMaterialDto>(libro);
                return libroDto;
            }
        }

    }
}
