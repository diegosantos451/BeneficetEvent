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
    public class BingoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BingoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Bingo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bingo>>> GetBingos()
        {
            return await _context.Bingos.ToListAsync();
        }

        // GET: api/Bingo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bingo>> GetBingo(Guid id)
        {
            var bingo = await _context.Bingos.FindAsync(id);

            if (bingo == null)
            {
                return NotFound();
            }

            return bingo;
        }

        // PUT: api/Bingo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBingo(Guid id, Bingo bingo)
        {
            if (id != bingo.Id)
            {
                return BadRequest();
            }

            _context.Entry(bingo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BingoExists(id))
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

        // POST: api/Bingo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bingo>> PostBingo(Bingo bingo)
        {
            _context.Bingos.Add(bingo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBingo", new { id = bingo.Id }, bingo);
        }

        // DELETE: api/Bingo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBingo(Guid id)
        {
            var bingo = await _context.Bingos.FindAsync(id);
            if (bingo == null)
            {
                return NotFound();
            }

            _context.Bingos.Remove(bingo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BingoExists(Guid id)
        {
            return _context.Bingos.Any(e => e.Id == id);
        }
    }
}
