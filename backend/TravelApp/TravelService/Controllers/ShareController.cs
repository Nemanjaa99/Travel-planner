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
    [Route("api/travel-plans/{planId}/share")]
    public class ShareController : ControllerBase
    {
        private readonly TravelDbContext _db;

        public ShareController(TravelDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateShareToken(int planId, [FromBody] CreateShareTokenDto dto)
        {
            var plan = await _db.TravelPlans.FindAsync(planId);
            if (plan == null) return NotFound();
            if (plan.UserId != GetUserId()) return Forbid();

            if (dto.AccessType != "view" && dto.AccessType != "edit")
                return BadRequest(new { message = "AccessType mora biti 'view' ili 'edit'." });

            var token = new ShareToken
            {
                TravelPlanId = planId,
                Token = Guid.NewGuid().ToString("N"),
                AccessType = dto.AccessType,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt
            };

            _db.ShareTokens.Add(token);
            await _db.SaveChangesAsync();

            return Ok(TravelMapper.ToDto(token));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTokens(int planId)
        {
            var plan = await _db.TravelPlans.FindAsync(planId);
            if (plan == null) return NotFound();
            if (plan.UserId != GetUserId()) return Forbid();

            var tokens = await _db.ShareTokens
                .Where(s => s.TravelPlanId == planId)
                .ToListAsync();

            return Ok(tokens.Select(TravelMapper.ToDto));
        }

        [Authorize]
        [HttpDelete("{tokenId}")]
        public async Task<IActionResult> RevokeToken(int planId, int tokenId)
        {
            var plan = await _db.TravelPlans.FindAsync(planId);
            if (plan == null) return NotFound();
            if (plan.UserId != GetUserId()) return Forbid();

            var token = await _db.ShareTokens
                .FirstOrDefaultAsync(s => s.Id == tokenId && s.TravelPlanId == planId);
            if (token == null) return NotFound();

            _db.ShareTokens.Remove(token);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/shared")]
    public class SharedPlanController : ControllerBase
    {
        private readonly TravelDbContext _db;

        public SharedPlanController(TravelDbContext db)
        {
            _db = db;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetSharedPlan(string token)
        {
            var shareToken = await _db.ShareTokens
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.Destinations)
                .Include(s => s.TravelPlan)
                    .ThenInclude(p => p.Activities)
                .FirstOrDefaultAsync(s => s.Token == token);

            if (shareToken == null)
                return NotFound(new { message = "Token nije pronađen." });

            if (shareToken.ExpiresAt.HasValue && shareToken.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "Token je istekao." });

            return Ok(new
            {
                accessType = shareToken.AccessType,
                plan = TravelMapper.ToDto(shareToken.TravelPlan)
            });
        }
    }
}