﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly IMediator _mediador;

        public AutorController(IMediator mediador) {
            _mediador = mediador;
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data) {
            return await _mediador.Send(data);
        }

        [HttpGet]
        public async Task<ActionResult<List<AutorDto>>> GetAutores()
        {
            return await _mediador.Send(new Consulta.ListaAutor());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AutorDto>> GetAutorUnico(string id)
        {
            return await _mediador.Send(new ConsultaFiltro.AutorUnico{AutorGuid = id});
        }

    }
}
