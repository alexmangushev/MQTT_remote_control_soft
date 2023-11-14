using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mqtt_remote_server.Models;

namespace mqtt_remote_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataController : ControllerBase
    {
        private readonly TelemetryContext _context;

        public DataController(TelemetryContext context)
        {
            _context = context;
        }

        // GET: api/Data/for_user
        [HttpPost("for_user")]
        public async Task<ActionResult<IEnumerable<Datum>>> GetData(RequestData date)
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault();
                string login = claim.Value;
                Console.WriteLine($"CLAIM TYPE: {claim.Type}; CLAIM VALUE: {login}");

                // Get data only for this user
                List<User> user = _context.Users.Where(r => r.Login == login).ToList();
                int? userid = user[0].UserId;
                List<DeviceToUser> devtouser = _context.DeviceToUsers.Where(r => r.UserId == userid).ToList();

                return _context.Data.Where(r => r.GetTime > date.Start && r.GetTime < date.End && r.DeviceId == devtouser[0].DeviceId).ToImmutableList();
            }

            if (_context.Data == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/Data
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Datum>>> GetData()
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            return await _context.Data.ToListAsync();
        }

        // GET: api/Data/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Datum>> GetDatum(int id)
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            var datum = await _context.Data.FindAsync(id);

            if (datum == null)
            {
                return NotFound();
            }

            return datum;
        }

        // PUT: api/Data/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatum(int id, Datum datum)
        {
            if (id != datum.Id)
            {
                return BadRequest();
            }

            _context.Entry(datum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatumExists(id))
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

        // POST: api/Data
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Datum>> PostDatum(Datum datum)
        {
            if (_context.Data == null)
            {
                return Problem("Entity set 'TelemetryContext.Data'  is null.");
            }
            _context.Data.Add(datum);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatum", new { id = datum.Id }, datum);
        }

        // DELETE: api/Data/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatum(int id)
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            var datum = await _context.Data.FindAsync(id);
            if (datum == null)
            {
                return NotFound();
            }

            _context.Data.Remove(datum);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DatumExists(int id)
        {
            return (_context.Data?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
