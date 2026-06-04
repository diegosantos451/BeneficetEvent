
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;
using BeneficentEvent.DTOs.Request;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenfeitorController : ControllerBase
    {
        private readonly BenfeitorService _benfeitorService;

        public BenfeitorController(BenfeitorService benfeitorService)
        {
            _benfeitorService = benfeitorService;
        }

        // GET: api/Benfeitor
        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var benfeitores = await _benfeitorService.ListarAsync();
            return Ok(benfeitores);
        }

        // GET: api/Benfeitor/5
        [HttpGet("{id}")]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var benfeitor = await _benfeitorService.BuscarPorIdAsync(id);

            if (benfeitor == null)
            {
                return NotFound();
            }

            return Ok(benfeitor);
        }

        // PUT: api/Benfeitor/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, CriarBenfeitorRequest request)
        {
            await _benfeitorService.EditarAsync(id, request);
            return Ok(new { Id = id, Mensagem = "Alterações feitas com sucesso." });
        }

        // POST: api/Benfeitor
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Benfeitor>> Criar(CriarBenfeitorRequest request)
        {
            var id = await _benfeitorService.CriarAsync(request);

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id = id, Mensagem = "Registro salvo com sucesso." });
        }

        // DELETE: api/Benfeitor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _benfeitorService.ExcluirAsync(id);

            return Ok(new {id = id, Mensagem="Registro apagado com sucesso."});
        }
    }
}
