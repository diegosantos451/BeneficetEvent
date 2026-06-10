
using Microsoft.AspNetCore.Mvc;

using BeneficentEvent.Services;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;


namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeilaoController : ControllerBase
    {
        private readonly LeilaoService _leilaoService;

        public LeilaoController(LeilaoService leilaoService)
        {
            _leilaoService = leilaoService;
        }

        // GET: api/Leilao
        [HttpGet]
        public async Task<ActionResult<List<LeilaoResponse>>> Listar()
        {
            return Ok(await _leilaoService.ListarAsync());
        }

        // GET: api/Leilao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeilaoDetalheResponse>> ObterPorIdDetalhes(Guid id)
        {   
            return Ok(await _leilaoService.ObterPorIdDetalhesAsync(id));
        }

        // PUT: api/Leilao/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, EditarLeilaoRequest request)
        {
            await _leilaoService.Editar(id, request);

            return NoContent();
        }

        
        // POST: api/Leilao
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> Criar(CriarLeilaoRequest request)
        {
            var id = await _leilaoService.CriarAsync(request);
            return Ok(new {id = id, Mensagem="Criado com sucesso!"});

        }
        
        // DELETE: api/Leilao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirAsync(Guid id)
        {
            await _leilaoService.ExcluirAsync(id);
            return Ok(new{Mensagem="Leilão apagado com sucesso!"});
        }
    }
}
