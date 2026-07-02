using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace KFH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Microsoft.AspNetCore.Authorization.Authorize]
    public class CustomerAccountController : ControllerBase
    {
        private readonly KFHContext _context;

        public CustomerAccountController(KFHContext context)
        {
            _context = context;
        }

        // POST: api/CustomerAccount
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerAccount>> CreateAccount([FromBody] CustomerAccount account)
        {
            if (account == null)
                return BadRequest();

            var exists = await _context.CustomerAccounts.AnyAsync(a => a.AccountNumber == account.AccountNumber);
            if (exists)
                return Conflict("Account with the same AccountNumber already exists.");

            account.CreatedDate = DateTime.UtcNow;

            _context.CustomerAccounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = account.AccountNumber }, account);
        }

        // GET: api/CustomerAccount/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerAccount>> GetById(int id)
        {
            var account = await _context.CustomerAccounts.FindAsync(id);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        // GET: api/CustomerAccount
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerAccount>>> GetAll()
        {
            var accounts = await _context.CustomerAccounts.ToListAsync();
            return Ok(accounts);
        }
    }
}
