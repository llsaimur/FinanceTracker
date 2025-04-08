using FinanceTrackerAPI.DTOs;
using FinanceTrackerAPI.Models;
using FinanceTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _service;

        public AccountsController(AccountService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var accounts = await _service.GetUserAccounts(userId);
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var account = await _service.GetAccount(id);
            if (account == null || account.UserId != GetUserId())
                return Forbid();

            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = new Account
            {
                Name = dto.Name,
                Type = dto.Type,
                Balance = dto.Balance,
                UserId = GetUserId()
            };

            try
            {
                await _service.CreateAccount(account);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var account = await _service.GetAccount(id);
                if (account == null || account.UserId != GetUserId())
                    return Forbid();

                account.Name = dto.Name;
                account.Type = dto.Type;
                account.Balance = dto.Balance;

                await _service.UpdateAccount(account);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var account = await _service.GetAccount(id);
            if (account == null || account.UserId != GetUserId())
                return Forbid();

            await _service.DeleteAccount(id);
            return NoContent(); 
        }
    }
}
