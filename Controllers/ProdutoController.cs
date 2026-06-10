
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;
using BeneficentEvent.Services;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutoController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        // GET: api/Produto
        [HttpGet]
        public async Task<ActionResult<List<ProdutoResponse>>> Listar()
        {
            return Ok(await _produtoService.ListarAsync());
        }

        // GET: api/Produto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDetalheResponse>> ObterPorIdDetalhe(Guid id)
        {
            return Ok(await _produtoService.ObterPorIdDetalheAsync(id));
        }

        // PUT: api/Produto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, CriarProdutoRequest request)
        {
            await _produtoService.EditarAsync(id, request);
            return NoContent();
        }

        // POST: api/Produto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Produto>> Criar(CriarProdutoRequest request)
        {
            var id = await _produtoService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorIdDetalhe), new { id = id }, new { Mensagem = "Registro salvo com sucesso." });
        }

        // DELETE: api/Produto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _produtoService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
