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
    [Route("api/travel-plans/{planId}/destinations")]
    [Authorize]
    public class DestinationsController : ControllerBase
    {
        private readonly TravelDbContext _db;

        public DestinationsController(TravelDbContext db)
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

        [HttpGet]
        public async Task<IActionResult> GetAll(int planId)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var destinations = await _db.Destinations
                .Where(d => d.TravelPlanId == planId)
                .ToListAsync();

            return Ok(destinations.Select(TravelMapper.ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int planId, int id)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var dest = await _db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == planId);
            if (dest == null) return NotFound();

            return Ok(TravelMapper.ToDto(dest));
        }

        [HttpPost]
        public async Task<IActionResult> Create(int planId, [FromBody] CreateDestinationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var dest = new Destination
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Location = dto.Location,
                ArrivalDate = dto.ArrivalDate,
                DepartureDate = dto.DepartureDate,
                Description = dto.Description,
                Notes = dto.Notes
            };

            _db.Destinations.Add(dest);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { planId, id = dest.Id }, TravelMapper.ToDto(dest));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int planId, int id, [FromBody] UpdateDestinationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var dest = await _db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == planId);
            if (dest == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name)) dest.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Location)) dest.Location = dto.Location;
            if (!string.IsNullOrWhiteSpace(dto.Description)) dest.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.Notes)) dest.Notes = dto.Notes;
            if (dto.ArrivalDate.HasValue) dest.ArrivalDate = dto.ArrivalDate;
            if (dto.DepartureDate.HasValue) dest.DepartureDate = dto.DepartureDate;

            await _db.SaveChangesAsync();
            return Ok(TravelMapper.ToDto(dest));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int planId, int id)
        {
            var plan = await GetPlanAsync(planId);
            if (plan == null) return NotFound();

            var dest = await _db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == planId);
            if (dest == null) return NotFound();

            _db.Destinations.Remove(dest);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}