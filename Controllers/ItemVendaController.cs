
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemVendaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemVendaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemVenda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemVenda>>> GetItensVenda()
        {
            return await _context.ItensVenda.ToListAsync();
        }

        // GET: api/ItemVenda/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemVenda>> GetItemVenda(Guid id)
        {
            var itemVenda = await _context.ItensVenda.FindAsync(id);

            if (itemVenda == null)
            {
                return NotFound();
            }

            return itemVenda;
        }

        // PUT: api/ItemVenda/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemVenda(Guid id, ItemVenda itemVenda)
        {
            if (id != itemVenda.Id)
            {
                return BadRequest();
            }

            _context.Entry(itemVenda).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemVendaExists(id))
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

        // POST: api/ItemVenda
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemVenda>> PostItemVenda(ItemVenda itemVenda)
        {
            _context.ItensVenda.Add(itemVenda);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemVenda", new { id = itemVenda.Id }, itemVenda);
        }

        // DELETE: api/ItemVenda/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemVenda(Guid id)
        {
            var itemVenda = await _context.ItensVenda.FindAsync(id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            _context.ItensVenda.Remove(itemVenda);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemVendaExists(Guid id)
        {
            return _context.ItensVenda.Any(e => e.Id == id);
        }
    }
}
