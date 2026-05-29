using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemDoacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemDoacaoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemDoacao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDoacao>>> GetItensDoacao()
        {
            return await _context.ItensDoacao.ToListAsync();
        }

        // GET: api/ItemDoacao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDoacao>> GetItemDoacao(Guid id)
        {
            var itemDoacao = await _context.ItensDoacao.FindAsync(id);

            if (itemDoacao == null)
            {
                return NotFound();
            }

            return itemDoacao;
        }

        // PUT: api/ItemDoacao/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemDoacao(Guid id, ItemDoacao itemDoacao)
        {
            if (id != itemDoacao.Id)
            {
                return BadRequest();
            }

            _context.Entry(itemDoacao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemDoacaoExists(id))
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

        // POST: api/ItemDoacao
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemDoacao>> PostItemDoacao(ItemDoacao itemDoacao)
        {
            _context.ItensDoacao.Add(itemDoacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemDoacao", new { id = itemDoacao.Id }, itemDoacao);
        }

        // DELETE: api/ItemDoacao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemDoacao(Guid id)
        {
            var itemDoacao = await _context.ItensDoacao.FindAsync(id);
            if (itemDoacao == null)
            {
                return NotFound();
            }

            _context.ItensDoacao.Remove(itemDoacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemDoacaoExists(Guid id)
        {
            return _context.ItensDoacao.Any(e => e.Id == id);
        }
    }
}
