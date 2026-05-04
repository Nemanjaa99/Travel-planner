using UserService.Dtos;
using UserService.Models;

namespace UserService.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }

        public static User FromRegisterDto(RegisterDto dto, string passwordHash)
        {
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "user",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}