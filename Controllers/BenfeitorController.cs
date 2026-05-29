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
    public class BenfeitorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BenfeitorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Benfeitor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Benfeitor>>> GetBenfeitores()
        {
            return await _context.Benfeitores.ToListAsync();
        }

        // GET: api/Benfeitor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Benfeitor>> GetBenfeitor(Guid id)
        {
            var benfeitor = await _context.Benfeitores.FindAsync(id);

            if (benfeitor == null)
            {
                return NotFound();
            }

            return benfeitor;
        }

        // PUT: api/Benfeitor/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBenfeitor(Guid id, Benfeitor benfeitor)
        {
            if (id != benfeitor.Id)
            {
                return BadRequest();
            }

            _context.Entry(benfeitor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenfeitorExists(id))
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

        // POST: api/Benfeitor
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Benfeitor>> PostBenfeitor(Benfeitor benfeitor)
        {
            _context.Benfeitores.Add(benfeitor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBenfeitor", new { id = benfeitor.Id }, benfeitor);
        }

        // DELETE: api/Benfeitor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenfeitor(Guid id)
        {
            var benfeitor = await _context.Benfeitores.FindAsync(id);
            if (benfeitor == null)
            {
                return NotFound();
            }

            _context.Benfeitores.Remove(benfeitor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BenfeitorExists(Guid id)
        {
            return _context.Benfeitores.Any(e => e.Id == id);
        }
    }
}
