using TravelService.Dtos;
using TravelService.Models;

namespace TravelService.Mappers
{
    public static class TravelMapper
    {
        public static TravelPlanDto ToDto(TravelPlan plan)
        {
            return new TravelPlanDto
            {
                Id = plan.Id,
                UserId = plan.UserId,
                Name = plan.Name,
                Description = plan.Description,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                PlannedBudget = plan.PlannedBudget,
                Notes = plan.Notes,
                CreatedAt = plan.CreatedAt,
                Destinations = plan.Destinations?.Select(ToDto).ToList() ?? new(),
                Activities = plan.Activities?.Select(ToDto).ToList() ?? new()
            };
        }

        public static DestinationDto ToDto(Destination d)
        {
            return new DestinationDto
            {
                Id = d.Id,
                TravelPlanId = d.TravelPlanId,
                Name = d.Name,
                Location = d.Location,
                ArrivalDate = d.ArrivalDate,
                DepartureDate = d.DepartureDate,
                Description = d.Description,
                Notes = d.Notes
            };
        }

        public static ActivityDto ToDto(Activity a)
        {
            return new ActivityDto
            {
                Id = a.Id,
                TravelPlanId = a.TravelPlanId,
                Name = a.Name,
                Date = a.Date,
                Time = a.Time.HasValue ? a.Time.Value.ToString(@"hh\:mm") : null,
                Location = a.Location,
                Description = a.Description,
                EstimatedCost = a.EstimatedCost,
                Status = a.Status
            };
        }

        public static ShareTokenDto ToDto(ShareToken s)
        {
            return new ShareTokenDto
            {
                Id = s.Id,
                TravelPlanId = s.TravelPlanId,
                Token = s.Token,
                AccessType = s.AccessType,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt
            };
        }
    }
}