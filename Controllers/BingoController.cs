
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
    public class BingoController : ControllerBase
    {
        private readonly BingoService _bingoService;

        public BingoController(BingoService bingoService)
        {
            _bingoService = bingoService;
        }

        // GET: api/Bingo
        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var bingos = await _bingoService.ListarAsync();
            return Ok(bingos);
        }

        // GET: api/Bingo/5
        [HttpGet("{id}")]
        public async Task<ActionResult> BuscarPorId(Guid id)
        {
            var bingo = await _bingoService.BuscarPorIdAsync(id);

            if (bingo == null)
            {
                return NotFound();
            }

            return Ok(bingo);
        }

        // PUT: api/Bingo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, EditarBingoRequest request)
        {
            await _bingoService.EditarAsync(id, request);
            return NoContent();
        }

        // POST: api/Bingo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bingo>> Criar(CriarBingoRequest request)
        {
            var id = await _bingoService.CriarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { Id = id }, new { Mensagem = "Registro salvo com sucesso." });
        }

        // DELETE: api/Bingo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _bingoService.ExcluirAsync(id);
            return Ok(new { Id = id, Mensagem = "Registro apagado com sucesso." });
        }
    }
}
