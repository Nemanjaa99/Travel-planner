using ExpenseService.Dtos;
using ExpenseService.Models;

namespace ExpenseService.Mappers
{
    public static class ExpenseMapper
    {
        public static ExpenseDto ToDto(Expense e)
        {
            return new ExpenseDto
            {
                Id = e.Id,
                TravelPlanId = e.TravelPlanId,
                UserId = e.UserId,
                Name = e.Name,
                Category = e.Category,
                Amount = e.Amount,
                Date = e.Date,
                Description = e.Description,
                CreatedAt = e.CreatedAt
            };
        }

        public static ChecklistItemDto ToDto(ChecklistItem c)
        {
            return new ChecklistItemDto
            {
                Id = c.Id,
                TravelPlanId = c.TravelPlanId,
                UserId = c.UserId,
                Name = c.Name,
                IsCompleted = c.IsCompleted,
                Order = c.Order,
                CreatedAt = c.CreatedAt
            };
        }
    }
}
