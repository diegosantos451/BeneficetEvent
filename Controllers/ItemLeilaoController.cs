
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemLeilaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemLeilaoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemLeilao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemLeilao>>> GetItensLeilao()
        {
            return await _context.ItensLeilao.ToListAsync();
        }

        // GET: api/ItemLeilao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemLeilao>> GetItemLeilao(Guid id)
        {
            var itemLeilao = await _context.ItensLeilao.FindAsync(id);

            if (itemLeilao == null)
            {
                return NotFound();
            }

            return itemLeilao;
        }

        // PUT: api/ItemLeilao/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemLeilao(Guid id, ItemLeilao itemLeilao)
        {
            if (id != itemLeilao.Id)
            {
                return BadRequest();
            }

            _context.Entry(itemLeilao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemLeilaoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ItemLeilao
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemLeilao>> PostItemLeilao(ItemLeilao itemLeilao)
        {
            _context.ItensLeilao.Add(itemLeilao);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemLeilao", new { id = itemLeilao.Id }, itemLeilao);
        }

        // DELETE: api/ItemLeilao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemLeilao(Guid id)
        {
            var itemLeilao = await _context.ItensLeilao.FindAsync(id);
            if (itemLeilao == null)
            {
                return NotFound();
            }

            _context.ItensLeilao.Remove(itemLeilao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemLeilaoExists(Guid id)
        {
            return _context.ItensLeilao.Any(e => e.Id == id);
        }
    }
}
