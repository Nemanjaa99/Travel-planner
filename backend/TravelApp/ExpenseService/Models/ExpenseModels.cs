using System.ComponentModel.DataAnnotations;

namespace ExpenseService.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TravelPlanId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ChecklistItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TravelPlanId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int Order { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}