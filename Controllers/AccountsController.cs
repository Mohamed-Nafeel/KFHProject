using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFH.Data;
using KFH.Models;

namespace KFH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AccountsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _db.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var acc = await _db.Accounts.FindAsync(id);
            if (acc == null) return NotFound();
            return Ok(acc);
        }

        public class CreateAccountRequest
        {
            public string CustomerName { get; set; } = null!;
            public decimal InitialBalance { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest req)
        {
            var account = new Account
            {
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0,12),
                CustomerName = req.CustomerName,
                CurrentBalance = req.InitialBalance,
                CreatedDate = DateTime.UtcNow,
                Status = "Active"
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }
    }
}
