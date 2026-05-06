using System.ComponentModel.DataAnnotations;

namespace ExpenseService.Dtos
{
    public class CreateExpenseDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Iznos mora biti veći od 0.")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }

    public class UpdateExpenseDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public string Category { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Iznos mora biti veći od 0.")]
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }

    public class ExpenseDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BudgetSummaryDto
    {
        public int TravelPlanId { get; set; }
        public decimal PlannedBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Remaining { get; set; }
        public Dictionary<string, decimal> ByCategory { get; set; }
    }

    public class CreateChecklistItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public int Order { get; set; } = 0;
    }

    public class UpdateChecklistItemDto
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public bool? IsCompleted { get; set; }
        public int? Order { get; set; }
    }

    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}