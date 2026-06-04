
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
    public class DespesaController : ControllerBase
    {
        private readonly DespesaService _despesaService;

        public DespesaController(DespesaService despesaService)
        {
            _despesaService = despesaService;
        }

        // GET: api/Despesa
        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var despesas = await _despesaService.ListarAsync();
            return Ok(despesas);
        }

        // GET: api/Despesa/5
        [HttpGet("{id}")]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var despesa = await _despesaService.ObterPorIdAsync(id);

            if (despesa == null)
            {
                return NotFound();
            }

            return Ok(despesa);
        }

        // PUT: api/Despesa/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, CriarDespesaRequest request)
        {
            await _despesaService.EditarAsync(id, request);
            return NoContent();
        }

        // POST: api/Despesa
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Despesa>> Criar(CriarDespesaRequest request)
        {
            var id = await _despesaService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { Id = id }, new { Mensagem = "Registro salvo com sucesso." });
        }

        // DELETE: api/Despesa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _despesaService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
