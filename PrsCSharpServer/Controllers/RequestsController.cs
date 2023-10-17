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
    public class RequestsController :ControllerBase {
        private readonly PrsDbContext _context;

        public RequestsController(PrsDbContext context) {
            _context = context;
        }

        // GET: api/Requests/review/{userId}
        [HttpGet("reviews/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsInReview(int userId) {
            if(_context.Requests == null) {
                return NotFound();
            }
            return await _context.Requests
                                    .Where(x => x.Status == "REVIEW" && x.UserId != userId)
                                    .Include(x => x.User)
                                    .ToListAsync();
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests() {
            if(_context.Requests == null) {
                return NotFound();
            }
            return await _context.Requests
                                    .Include(x => x.User)
                                    .ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id) {
            if(_context.Requests == null) {
                return NotFound();
            }
            var request = await _context.Requests
                                        .Include(x => x.User)
                                        .Include(x => x.Requestlines)
                                        .ThenInclude(x => x.Product)
                                        .SingleOrDefaultAsync(x => x.Id == id);

            if(request == null) {
                return NotFound();
            }

            return request;
        }

        [HttpPut("review/{id}")]
        public async Task<IActionResult> ReviewRequest(int id, Request request) {
            request.Status = (request.Total <= 50) ? "APPROVED" : "REVIEW";
            request.RejectionReason = null;
            return await PutRequest(id, request);
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id, Request request) {
            request.Status = "APPROVED";
            request.RejectionReason = null;
            return await PutRequest(id, request);
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id, Request request) {
            request.Status = "REJECTED";
            return await PutRequest(id, request);
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request) {
            if(id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch(DbUpdateConcurrencyException) {
                if(!RequestExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request) {
            if(_context.Requests == null) {
                return Problem("Entity set 'PrsDbContext.Requests'  is null.");
            }
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id) {
            if(_context.Requests == null) {
                return NotFound();
            }
            var request = await _context.Requests.FindAsync(id);
            if(request == null) {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id) {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
