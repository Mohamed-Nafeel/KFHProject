using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFH.Data;
using KFH.Models;

namespace KFH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TransfersController> _logger;

        public TransfersController(AppDbContext db, ILogger<TransfersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transfers = await _db.TransferRequests.Include(t => t.SourceAccount).Include(t => t.DestinationAccount).ToListAsync();
            return Ok(transfers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _db.TransferRequests.Include(x => x.SourceAccount).Include(x => x.DestinationAccount).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        public class CreateTransferRequestDto
        {
            public int SourceAccountId { get; set; }
            public int DestinationAccountId { get; set; }
            public decimal Amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferRequestDto dto)
        {
            // basic validation
            if (dto.Amount <= 0) return BadRequest("Amount must be positive");
            if (dto.SourceAccountId == dto.DestinationAccountId) return BadRequest("Source and destination must differ");

            var src = await _db.Accounts.FindAsync(dto.SourceAccountId);
            var dst = await _db.Accounts.FindAsync(dto.DestinationAccountId);
            if (src == null || dst == null) return BadRequest("Source or destination account not found");

            var tr = new TransferRequest
            {
                SourceAccountId = dto.SourceAccountId,
                DestinationAccountId = dto.DestinationAccountId,
                Amount = dto.Amount,
                CreatedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _db.TransferRequests.Add(tr);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = tr.Id }, tr);
        }

        [HttpPost("{id:int}/process")]
        public async Task<IActionResult> Process(int id)
        {
            var tr = await _db.TransferRequests.Include(x => x.SourceAccount).Include(x => x.DestinationAccount).FirstOrDefaultAsync(x => x.Id == id);
            if (tr == null) return NotFound();
            if (tr.Status != "Pending") return BadRequest("Transfer already processed");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var src = await _db.Accounts.FindAsync(tr.SourceAccountId);
                var dst = await _db.Accounts.FindAsync(tr.DestinationAccountId);
                if (src == null || dst == null)
                {
                    tr.Status = "Failed";
                    await _db.SaveChangesAsync();
                    await tx.RollbackAsync();
                    return BadRequest("Accounts not found");
                }

                if (src.CurrentBalance < tr.Amount)
                {
                    tr.Status = "Failed";
                    await _db.SaveChangesAsync();
                    await tx.RollbackAsync();
                    return BadRequest("Insufficient funds");
                }

                src.CurrentBalance -= tr.Amount;
                dst.CurrentBalance += tr.Amount;
                tr.Status = "Completed";

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(tr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transfer {Id}", id);
                tr.Status = "Failed";
                await _db.SaveChangesAsync();
                await tx.RollbackAsync();
                return StatusCode(500, "Error processing transfer");
            }
        }
    }
}
