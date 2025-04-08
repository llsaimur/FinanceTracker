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
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _service;

        public TransactionsController(TransactionService service)
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
            var transactions = await _service.GetUserTransactions(userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var transaction = await _service.GetTransaction(id);
            if (transaction == null || transaction.UserId != GetUserId())
                return Forbid();

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = new Transaction
            {
                UserId = GetUserId(),
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Category = dto.Category,
                Date = dto.Date,
                Description = dto.Description
            };

            try
            {
                await _service.CreateTransaction(transaction);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _service.GetTransaction(id);
            if (transaction == null || transaction.UserId != GetUserId())
                return Forbid();

            transaction.AccountId = dto.AccountId;
            transaction.Amount = dto.Amount;
            transaction.Category = dto.Category;
            transaction.Date = dto.Date;
            transaction.Description = dto.Description;

            try
            {
                await _service.UpdateTransaction(transaction);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var transaction = await _service.GetTransaction(id);
            if (transaction == null || transaction.UserId != GetUserId())
                return Forbid();

            try
            {
                await _service.DeleteTransaction(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
