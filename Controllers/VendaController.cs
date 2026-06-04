using Microsoft.AspNetCore.Mvc;
using BeneficentEvent.Models;
using BeneficentEvent.Services;
using BeneficentEvent.DTOs.Request;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private readonly VendaService _vendaService;

        public VendaController(VendaService vendaService)
        {
            _vendaService = vendaService;
        }

        // GET: api/Venda
        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var vendas = await _vendaService.ListarAsync();
            return Ok(vendas);
        }

        // GET: api/Venda/5
        [HttpGet("{id}")]
        public async Task<ActionResult> BuscarPorId(Guid id)
        {
            var venda = await _vendaService.BuscarPorIdAsync(id);

            if (venda == null)
            {
                return NotFound();
            }

            return Ok(venda);
        }

        // PUT: api/Venda/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id,EditarVendaRequest request)
        {
            await _vendaService.EditarAsync(id,request);
            return Ok(new {Mensagem="Venda alterada com sucesso."});
        }

        // POST: api/Venda
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Venda>> Criar(CriarVendaRequest request)
        {
            var id = await _vendaService.CriarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId),new { id },new { Id = id, Mensagem ="Registro salvo com sucesso." }
);
        }

        // DELETE: api/Venda/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(Guid id)
        {
            await _vendaService.ExcluirAsync(id);
            return Ok(new {Mensagem="Venda apagada com sucesso."});
        }
    }
}
