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
    public class BudgetsController : ControllerBase
    {
        private readonly BudgetService _service;

        public BudgetsController(BudgetService service)
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
            var budgets = await _service.GetUserBudgets(GetUserId());
            return Ok(budgets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var budget = await _service.GetBudget(id);
            if (budget == null || budget.UserId != GetUserId())
                return Forbid();

            return Ok(budget);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BudgetDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var budget = new Budget
            {
                UserId = GetUserId(),
                Category = dto.Category,
                Limit = dto.Limit,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            try
            {
                await _service.CreateBudget(budget);
                return Ok(budget);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BudgetDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var budget = await _service.GetBudget(id);
            if (budget == null || budget.UserId != GetUserId())
                return Forbid();

            budget.Category = dto.Category;
            budget.Limit = dto.Limit;
            budget.StartDate = dto.StartDate;
            budget.EndDate = dto.EndDate;

            try
            {
                await _service.UpdateBudget(budget);
                return Ok(budget);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var budget = await _service.GetBudget(id);
            if (budget == null || budget.UserId != GetUserId())
                return Forbid();

            await _service.DeleteBudget(id);
            return NoContent();
        }
    }
}
