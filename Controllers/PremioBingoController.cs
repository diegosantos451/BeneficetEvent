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
    public class PremioBingoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PremioBingoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PremioBingo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PremioBingo>>> GetPremiosBingo()
        {
            return await _context.PremiosBingo.ToListAsync();
        }

        // GET: api/PremioBingo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PremioBingo>> GetPremioBingo(Guid id)
        {
            var premioBingo = await _context.PremiosBingo.FindAsync(id);

            if (premioBingo == null)
            {
                return NotFound();
            }

            return premioBingo;
        }

        // PUT: api/PremioBingo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPremioBingo(Guid id, PremioBingo premioBingo)
        {
            if (id != premioBingo.Id)
            {
                return BadRequest();
            }

            _context.Entry(premioBingo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PremioBingoExists(id))
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

        // POST: api/PremioBingo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PremioBingo>> PostPremioBingo(PremioBingo premioBingo)
        {
            _context.PremiosBingo.Add(premioBingo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPremioBingo", new { id = premioBingo.Id }, premioBingo);
        }

        // DELETE: api/PremioBingo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePremioBingo(Guid id)
        {
            var premioBingo = await _context.PremiosBingo.FindAsync(id);
            if (premioBingo == null)
            {
                return NotFound();
            }

            _context.PremiosBingo.Remove(premioBingo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PremioBingoExists(Guid id)
        {
            return _context.PremiosBingo.Any(e => e.Id == id);
        }
    }
}
