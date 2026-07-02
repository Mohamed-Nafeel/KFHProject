using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KFH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Microsoft.AspNetCore.Authorization.Authorize]
    public class TransferRequestController : ControllerBase
    {
        private readonly KFHContext _context;

        public TransferRequestController(KFHContext context)
        {
            _context = context;
        }

        // POST: api/TransferRequest
        [HttpPost]
        public async Task<ActionResult<TransferRequest>> CreateTransfer([FromBody] TransferRequest request)
        {
            if (request == null)
                return BadRequest();

            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            // Ensure accounts exist
            var src = await _context.CustomerAccounts.FindAsync(request.SourceAccount);
            var dst = await _context.CustomerAccounts.FindAsync(request.DestinationAccount);
            if (src == null || dst == null)
                return BadRequest("Source or destination account does not exist.");

                        
            request.Status = "pending";

            _context.TransferRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { source = request.SourceAccount, destination = request.DestinationAccount, createdDate = request.CreatedDate }, request);
        }

        // GET: api/TransferRequest/{source}/{destination}/{createdDate}
        [HttpGet("{source}/{destination}/{createdDate}")]
        public async Task<ActionResult<TransferRequest>> GetById(int source, int destination, DateTime createdDate)
        {
            var tr = await _context.TransferRequests.FindAsync(source, destination, createdDate);
            if (tr == null)
                return NotFound();
            return Ok(tr);
        }

        // GET: api/TransferRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferRequest>>> ListTransfers()
        {
            var list = await _context.TransferRequests.ToListAsync();
            return Ok(list);
        }

        // POST: api/TransferRequest/{source}/{destination}/{createdDate}/process
        [HttpPost("{source}/{destination}/{createdDate}/process")]
        //[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult<TransferRequest>> ProcessTransfer(int source, int destination, DateTime createdDate)
        {
            var tr = await _context.TransferRequests.FindAsync(source, destination, createdDate);
            if (tr == null)
                return NotFound();

            if (!string.Equals(tr.Status, "pending", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Transfer already processed or invalid status.");

            var src = await _context.CustomerAccounts.FindAsync(tr.SourceAccount);
            var dst = await _context.CustomerAccounts.FindAsync(tr.DestinationAccount);
            if (src == null || dst == null)
            {
                tr.Status = "failed";
                await _context.SaveChangesAsync();
                return BadRequest("Source or destination account not found.");
            }

            if (src.CurrentBalance < tr.Amount)
            {
                tr.Status = "failed";
                await _context.SaveChangesAsync();
                return BadRequest("Insufficient funds in source account.");
            }

            // Perform transfer
            src.CurrentBalance -= tr.Amount;
            dst.CurrentBalance += tr.Amount;
            tr.Status = "completed";

            await _context.SaveChangesAsync();

            return Ok(tr);
        }
    }
}
