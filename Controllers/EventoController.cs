
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;
using BeneficentEvent.Services;
using BeneficentEvent.DTOs.Request;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _eventoService;

        public EventoController(EventoService eventoService)
        {
            _eventoService = eventoService;
        }

        // GET: api/Evento
        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var eventos = await _eventoService.ListarAsync();
            return Ok(eventos);
        }

        // GET: api/Evento/5
        [HttpGet("{id}")]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var evento = await _eventoService.ObterPorIdAsync(id);

            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        // PUT: api/Evento/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, CriarEventoRequest request)
        {
            await _eventoService.EditarAsync(id, request);
            return NoContent();
        }

        // POST: api/Evento
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Evento>> Criar(CriarEventoRequest request)
        {
            var id = await _eventoService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { Id = id }, new { Mensagem = "Registro salvo com sucesso." });
        }

        // DELETE: api/Evento/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _eventoService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
