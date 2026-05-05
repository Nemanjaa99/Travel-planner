using System.ComponentModel.DataAnnotations;

namespace TravelService.Dtos
{
    public class CreateTravelPlanDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Budžet ne može biti negativan.")]
        public decimal PlannedBudget { get; set; }
        [MaxLength(2000)]
        public string Notes { get; set; }
    }

    public class UpdateTravelPlanDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Budžet ne može biti negativan.")]
        public decimal? PlannedBudget { get; set; }
        [MaxLength(2000)]
        public string Notes { get; set; }
    }

    public class TravelPlanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PlannedBudget { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }

    public class CreateDestinationDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
    }

    public class UpdateDestinationDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
    }

    public class DestinationDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }

    public class CreateActivityDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Time { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? EstimatedCost { get; set; }
        public string Status { get; set; } = "planned";
    }

    public class UpdateActivityDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Time { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? EstimatedCost { get; set; }
        public string Status { get; set; }
    }

    public class ActivityDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string Status { get; set; }
    }

    public class CreateShareTokenDto
    {
        [Required]
        public string AccessType { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class ShareTokenDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public string Token { get; set; }
        public string AccessType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}