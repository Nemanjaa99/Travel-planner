using System.ComponentModel.DataAnnotations;

namespace TravelService.Models
{
    public class TravelPlan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public decimal PlannedBudget { get; set; }
        [MaxLength(2000)]
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public ICollection<ShareToken> ShareTokens { get; set; } = new List<ShareToken>();
    }

    public class Destination
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TravelPlanId { get; set; }
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
        public TravelPlan TravelPlan { get; set; }
    }

    public class Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TravelPlanId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public TimeSpan? Time { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public decimal? EstimatedCost { get; set; }
        [Required]
        public string Status { get; set; } = "planned";
        public TravelPlan TravelPlan { get; set; }
    }

    public class ShareToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TravelPlanId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string AccessType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public TravelPlan TravelPlan { get; set; }
    }
}