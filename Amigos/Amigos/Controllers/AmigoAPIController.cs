using Amigos.DataAccessLayer;
using Amigos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Amigos.Controllers
{
    [Route("api/amigo")]
    [ApiController]
    public class AmigoAPIController : ControllerBase
    {
        private readonly AmigoDBContext _context;

        public AmigoAPIController(AmigoDBContext context)
        {
            _context = context;
        }

        // GET: api/amigo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Amigo>>> GetAmigos()
        {
            return await _context.Amigos!.ToListAsync();
        }

        // GET: api/amigo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Amigo>> GetAmigo(int id)
        {
            var amigo = await _context.Amigos!.FindAsync(id);

            if (amigo == null)
            {
                return NotFound();
            }

            return amigo;
        }

        // PUT: api/amigo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAmigo(int id, Amigo amigo)
        {
            if (id != amigo.ID)
            {
                return BadRequest();
            }

            _context.Entry(amigo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmigoExists(id))
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

        // PUT: api/amigo/byname/{name}
        [HttpPut("byname/{name}")]
        public async Task<IActionResult> PutAmigoByName(string name, Amigo amigo)
        {
            var amigoDB = await _context.Amigos!.FirstOrDefaultAsync(a => a.name == name);
            if (amigoDB == null)
            {
                return NotFound();
            }
            amigoDB.lati = amigo.lati;
            amigoDB.longi = amigo.longi;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/amigo
        [HttpPost]
        public async Task<ActionResult<Amigo>> PostAmigo(Amigo amigo)
        {
            _context.Amigos!.Add(amigo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAmigo), new { id = amigo.ID }, amigo);
        }

        // DELETE: api/amigo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAmigo(int id)
        {
            var amigo = await _context.Amigos!.FindAsync(id);
            if (amigo == null)
            {
                return NotFound();
            }

            _context.Amigos!.Remove(amigo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AmigoExists(int id)
        {
            return _context.Amigos!.Any(e => e.ID == id);
        }
    }
}