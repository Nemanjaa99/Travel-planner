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
    [Route("api/travel-plans/{planId}/activities")]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly TravelDbContext _db;

        public ActivitiesController(TravelDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private bool IsAdmin() =>
            User.FindFirst(ClaimTypes.Role)?.Value == "admin";

        private async Task<TravelPlan?> GetPlanAsync(int planId)
        {
            var plan = await _db.TravelPlans.FindAsync(planId);
            if (plan == null) return null;
            if (!IsAdmin() && plan.UserId != GetUserId()) return null;
            return plan;
        }

        private static readonly string[] ValidStatuses =
            { "planned", "reserved", "completed", "cancelled" };

        [HttpGet]
        public async Task<IActionResult> GetAll(int planId)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var activities = await _db.Activities
                .Where(a => a.TravelPlanId == planId)
                .OrderBy(a => a.Date).ThenBy(a => a.Time)
                .ToListAsync();

            return Ok(activities.Select(TravelMapper.ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int planId, int id)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var act = await _db.Activities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == planId);
            if (act == null) return NotFound();

            return Ok(TravelMapper.ToDto(act));
        }

        [HttpPost]
        public async Task<IActionResult> Create(int planId, [FromBody] CreateActivityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            if (!ValidStatuses.Contains(dto.Status))
                return BadRequest(new { message = "Nevažeći status aktivnosti." });

            TimeSpan? time = null;
            if (!string.IsNullOrWhiteSpace(dto.Time) && TimeSpan.TryParse(dto.Time, out var parsed))
                time = parsed;

            var act = new Activity
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Date = dto.Date,
                Time = time,
                Location = dto.Location,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                Status = dto.Status
            };

            _db.Activities.Add(act);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { planId, id = act.Id }, TravelMapper.ToDto(act));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int planId, int id, [FromBody] UpdateActivityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var act = await _db.Activities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == planId);
            if (act == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name)) act.Name = dto.Name;
            if (dto.Date.HasValue) act.Date = dto.Date.Value;
            if (!string.IsNullOrWhiteSpace(dto.Location)) act.Location = dto.Location;
            if (!string.IsNullOrWhiteSpace(dto.Description)) act.Description = dto.Description;
            if (dto.EstimatedCost.HasValue) act.EstimatedCost = dto.EstimatedCost;
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (!ValidStatuses.Contains(dto.Status))
                    return BadRequest(new { message = "Nevažeći status aktivnosti." });
                act.Status = dto.Status;
            }
            if (!string.IsNullOrWhiteSpace(dto.Time) && TimeSpan.TryParse(dto.Time, out var parsed))
                act.Time = parsed;

            await _db.SaveChangesAsync();
            return Ok(TravelMapper.ToDto(act));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int planId, int id)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var act = await _db.Activities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == planId);
            if (act == null) return NotFound();

            _db.Activities.Remove(act);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}