using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PrsCSharpServer.Data;
using PrsCSharpServer.Models;

namespace PrsCSharpServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RequestlinesController :ControllerBase {
        private readonly PrsDbContext _context;

        public RequestlinesController(PrsDbContext context) {
            _context = context;
        }

        private async Task RecalculateRequestTotal(int requestId) {
            var request = await _context.Requests.FindAsync(requestId);
            if(request == null) { throw new Exception($"Request not found to recalculate! id={requestId}"); }
            request.Total = (from rl in _context.Requestlines
                             join p in _context.Products
                                 on rl.ProductId equals p.Id
                             where rl.RequestId == requestId
                             select new {
                                 LineTotal = rl.Quantity * p.Price
                             })
                            .Sum(x => x.LineTotal);
            await _context.SaveChangesAsync();
        }

        // GET: api/Requestlines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requestline>>> GetRequestlines() {
            if(_context.Requestlines == null) {
                return NotFound();
            }
            return await _context.Requestlines
                                    .Include(x => x.Product)
                                    .ToListAsync();
        }

        // GET: api/Requestlines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requestline>> GetRequestline(int id) {
            if(_context.Requestlines == null) {
                return NotFound();
            }
            var requestline = await _context.Requestlines
                                                .Include(x => x.Product)
                                                .SingleOrDefaultAsync(x => x.Id == id);

            if(requestline == null) {
                return NotFound();
            }

            return requestline;
        }

        // PUT: api/Requestlines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestline(int id, Requestline requestline) {
            if(id != requestline.Id) {
                return BadRequest();
            }

            _context.Entry(requestline).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                await RecalculateRequestTotal(requestline.RequestId);
            } catch(DbUpdateConcurrencyException) {
                if(!RequestlineExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requestlines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Requestline>> PostRequestline(Requestline requestline) {
            if(_context.Requestlines == null) {
                return Problem("Entity set 'PrsDbContext.Requestlines'  is null.");
            }
            _context.Requestlines.Add(requestline);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestline.RequestId);


            return CreatedAtAction("GetRequestline", new { id = requestline.Id }, requestline);
        }

        // DELETE: api/Requestlines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestline(int id) {
            if(_context.Requestlines == null) {
                return NotFound();
            }
            var requestline = await _context.Requestlines.FindAsync(id);
            if(requestline == null) {
                return NotFound();
            }

            var requestId = requestline.RequestId;
            _context.Requestlines.Remove(requestline);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestId);

            return NoContent();
        }

        private bool RequestlineExists(int id) {
            return (_context.Requestlines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
