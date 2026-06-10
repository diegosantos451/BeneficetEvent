
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
            return Ok(await _eventoService.ListarAsync());
        }

        // GET: api/Evento/5
        [HttpGet("{id}")]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            return Ok(await _eventoService.ObterPorIdDetalhesAsync(id));
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

        //POST: api/Evento/{idEvento}/Benfeitor/{id}
        [HttpPost("{eventoId}/Benfeitor/")]
        public async Task<IActionResult> AdicionarBenfeitor(Guid eventoId, CriarParticipanteEventoRequest request)
        {
            await _eventoService.AddBenfeitorAsync(eventoId, request);
            return NoContent();
        }
        // DELETE: api/Evento/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _eventoService.ExcluirAsync(id);
            return NoContent();
        }

        // DELETE: api/Evento/{eventoId}/Benfeitor/{benfeitorId}
        [HttpDelete("{eventoId}/Benfeitor/{benfeitorId}")]
        public async Task<IActionResult> RemoverBenfeitor(Guid eventoId, Guid benfeitorId)
        {
            await _eventoService.RemoverBenfeitorAsync(eventoId, benfeitorId);
            return NoContent();
        }

        [HttpGet("{eventoId}/Benfeitor")]
        public async Task<IActionResult> ListarParticipantesAsync(Guid eventoId)
        {
            var participantes = await _eventoService.ListarParticipantesAsync(eventoId);
            return Ok(participantes);
        }

        [HttpPost("{eventoId}/encerramento")]
        public async Task<IActionResult> EncerrarEvento(Guid eventoId)
        {
            await _eventoService.EncerrarEventoAsync(eventoId);
            return NoContent();
        }

        [HttpPost("{eventoId}/fechamentoBingo/{bingoId}/{quantidade}")]
        public async Task<IActionResult> FechamentoDeBingo(Guid eventoId, Guid bingoId, int quantidade)
        {
            await _eventoService.FechamentoDeBingoAsync(eventoId, bingoId, quantidade);

            return NoContent();
        }
    }
}
