using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelService.Data;
using TravelService.Dtos;
using TravelService.Mappers;
using TravelService.Models;

namespace TravelService.Controllers
{
    [ApiController]
    [Route("api/travel-plans")]
    [Authorize]
    public class TravelPlansController : ControllerBase
    {
        private readonly TravelDbContext _db;

        public TravelPlansController(TravelDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private bool IsAdmin() =>
            User.FindFirst(ClaimTypes.Role)?.Value == "admin";

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            IQueryable<TravelPlan> query = _db.TravelPlans
                .Include(t => t.Destinations)
                .Include(t => t.Activities);

            if (!IsAdmin())
                query = query.Where(t => t.UserId == userId);

            var plans = await query.ToListAsync();
            return Ok(plans.Select(TravelMapper.ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _db.TravelPlans
                .Include(t => t.Destinations)
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (plan == null) return NotFound();

            var userId = GetUserId();
            if (!IsAdmin() && plan.UserId != userId) return Forbid();

            return Ok(TravelMapper.ToDto(plan));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTravelPlanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.EndDate < dto.StartDate)
                return BadRequest(new { message = "Krajnji datum ne može biti prije početnog datuma." });

            var plan = new TravelPlan
            {
                UserId = GetUserId(),
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PlannedBudget = dto.PlannedBudget,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _db.TravelPlans.Add(plan);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, TravelMapper.ToDto(plan));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTravelPlanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = await _db.TravelPlans.FindAsync(id);
            if (plan == null) return NotFound();

            var userId = GetUserId();
            if (!IsAdmin() && plan.UserId != userId) return Forbid();

            if (!string.IsNullOrWhiteSpace(dto.Name)) plan.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) plan.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.Notes)) plan.Notes = dto.Notes;
            if (dto.PlannedBudget.HasValue) plan.PlannedBudget = dto.PlannedBudget.Value;
            if (dto.StartDate.HasValue) plan.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) plan.EndDate = dto.EndDate.Value;

            if (plan.EndDate < plan.StartDate)
                return BadRequest(new { message = "Krajnji datum ne može biti prije početnog datuma." });

            await _db.SaveChangesAsync();
            return Ok(TravelMapper.ToDto(plan));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _db.TravelPlans.FindAsync(id);
            if (plan == null) return NotFound();

            var userId = GetUserId();
            if (!IsAdmin() && plan.UserId != userId) return Forbid();

            _db.TravelPlans.Remove(plan);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}