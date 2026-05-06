using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ExpenseService.Data;
using ExpenseService.Dtos;
using ExpenseService.Mappers;
using ExpenseService.Models;

namespace ExpenseService.Controllers
{
    [ApiController]
    [Route("api/travel-plans/{planId}/expenses")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly ExpenseDbContext _db;

        public ExpensesController(ExpenseDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private static readonly string[] ValidCategories =
            { "transport", "accommodation", "food", "tickets", "shopping", "other" };

        [HttpGet]
        public async Task<IActionResult> GetAll(int planId)
        {
            var userId = GetUserId();
            var expenses = await _db.Expenses
                .Where(e => e.TravelPlanId == planId && e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return Ok(expenses.Select(ExpenseMapper.ToDto));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(int planId, [FromQuery] decimal plannedBudget = 0)
        {
            var userId = GetUserId();
            var expenses = await _db.Expenses
                .Where(e => e.TravelPlanId == planId && e.UserId == userId)
                .ToListAsync();

            var totalSpent = expenses.Sum(e => e.Amount);
            var byCategory = expenses
                .GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            return Ok(new BudgetSummaryDto
            {
                TravelPlanId = planId,
                PlannedBudget = plannedBudget,
                TotalSpent = totalSpent,
                Remaining = plannedBudget - totalSpent,
                ByCategory = byCategory
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int planId, int id)
        {
            var userId = GetUserId();
            var expense = await _db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == planId && e.UserId == userId);
            if (expense == null) return NotFound();
            return Ok(ExpenseMapper.ToDto(expense));
        }

        [HttpPost]
        public async Task<IActionResult> Create(int planId, [FromBody] CreateExpenseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!ValidCategories.Contains(dto.Category))
                return BadRequest(new { message = "Nevažeća kategorija troška." });

            var expense = new Expense
            {
                TravelPlanId = planId,
                UserId = GetUserId(),
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Date = dto.Date,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { planId, id = expense.Id }, ExpenseMapper.ToDto(expense));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int planId, int id, [FromBody] UpdateExpenseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var expense = await _db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == planId && e.UserId == userId);
            if (expense == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name)) expense.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) expense.Description = dto.Description;
            if (dto.Amount.HasValue) expense.Amount = dto.Amount.Value;
            if (dto.Date.HasValue) expense.Date = dto.Date.Value;
            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                if (!ValidCategories.Contains(dto.Category))
                    return BadRequest(new { message = "Nevažeća kategorija troška." });
                expense.Category = dto.Category;
            }

            await _db.SaveChangesAsync();
            return Ok(ExpenseMapper.ToDto(expense));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int planId, int id)
        {
            var userId = GetUserId();
            var expense = await _db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == planId && e.UserId == userId);
            if (expense == null) return NotFound();

            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
