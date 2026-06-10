
using Microsoft.AspNetCore.Mvc;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Services;
using BeneficentEvent.DTOs.Response;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoacaoController : ControllerBase
    {
        private readonly DoacaoService _doacaoService;

        public DoacaoController(DoacaoService doacaoService)
        {
            _doacaoService = doacaoService;
        }

        // GET: api/Doacao
        [HttpGet]
        public async Task<ActionResult<List<DoacaoResponse>>> Listar()
        {
            return Ok(await _doacaoService.ListarAsync());
        }

        // GET: api/Doacao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoacaoDetalheResponse>> ObterPorIdDetalhe(Guid id)
        {
            return Ok(await _doacaoService.ObterPorIdDetalheAsync(id));

        }

        // PUT: api/Doacao/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, EditarDoacaoRequest request)
        {
            await _doacaoService.EditarAsync(id, request);
            return Ok(new { mensagem = "Alterações salvas!" });
        }

        // POST: api/Doacao
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> Criar(CriarDoacaoRequest request)
        {
            var id = await _doacaoService.CriarAsync(request);
            return Ok(new { Id = id, Mensagem = "Doação cadastrada com sucesso!" });
        }

        // DELETE: api/Doacao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _doacaoService.ExcluirAsync(id);
            return Ok(new { mensagem = "Doação apagada com sucesso!" });
        }
    }
}
