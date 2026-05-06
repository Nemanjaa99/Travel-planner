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
    [Route("api/travel-plans/{planId}/checklist")]
    [Authorize]
    public class ChecklistController : ControllerBase
    {
        private readonly ExpenseDbContext _db;

        public ChecklistController(ExpenseDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll(int planId)
        {
            var userId = GetUserId();
            var items = await _db.ChecklistItems
                .Where(c => c.TravelPlanId == planId && c.UserId == userId)
                .OrderBy(c => c.Order).ThenBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(items.Select(ExpenseMapper.ToDto));
        }

        [HttpPost]
        public async Task<IActionResult> Create(int planId, [FromBody] CreateChecklistItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = new ChecklistItem
            {
                TravelPlanId = planId,
                UserId = GetUserId(),
                Name = dto.Name,
                IsCompleted = false,
                Order = dto.Order,
                CreatedAt = DateTime.UtcNow
            };

            _db.ChecklistItems.Add(item);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { planId }, ExpenseMapper.ToDto(item));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int planId, int id, [FromBody] UpdateChecklistItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var item = await _db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == planId && c.UserId == userId);
            if (item == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name)) item.Name = dto.Name;
            if (dto.IsCompleted.HasValue) item.IsCompleted = dto.IsCompleted.Value;
            if (dto.Order.HasValue) item.Order = dto.Order.Value;

            await _db.SaveChangesAsync();
            return Ok(ExpenseMapper.ToDto(item));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int planId, int id)
        {
            var userId = GetUserId();
            var item = await _db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == planId && c.UserId == userId);
            if (item == null) return NotFound();

            _db.ChecklistItems.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}