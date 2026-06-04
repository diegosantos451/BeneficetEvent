
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Data;
using BeneficentEvent.Models;

namespace BeneficentEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lance>>> GetLances()
        {
            return await _context.Lances.ToListAsync();
        }

        // GET: api/Lance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lance>> GetLance(Guid id)
        {
            var lance = await _context.Lances.FindAsync(id);

            if (lance == null)
            {
                return NotFound();
            }

            return lance;
        }

        // PUT: api/Lance/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLance(Guid id, Lance lance)
        {
            if (id != lance.Id)
            {
                return BadRequest();
            }

            _context.Entry(lance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanceExists(id))
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

        // POST: api/Lance
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lance>> PostLance(Lance lance)
        {
            _context.Lances.Add(lance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLance", new { id = lance.Id }, lance);
        }

        // DELETE: api/Lance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLance(Guid id)
        {
            var lance = await _context.Lances.FindAsync(id);
            if (lance == null)
            {
                return NotFound();
            }

            _context.Lances.Remove(lance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanceExists(Guid id)
        {
            return _context.Lances.Any(e => e.Id == id);
        }
    }
}
