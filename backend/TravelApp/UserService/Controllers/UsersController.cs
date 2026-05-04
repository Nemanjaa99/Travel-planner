using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Helpers;
using UserService.Mappers;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _db;
        private readonly JwtHelper _jwt;

        public UsersController(UserDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) return Conflict(new { message = "Korisnik sa ovim emailom već postoji." });

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = UserMapper.FromRegisterDto(dto, hash);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user);
            return Ok(new AuthResponseDto { Token = token, User = UserMapper.ToDto(user) });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Pogrešan email ili lozinka." });

            if (!user.IsActive) return Unauthorized(new { message = "Nalog je deaktiviran." });

            var token = _jwt.GenerateToken(user);
            return Ok(new AuthResponseDto { Token = token, User = UserMapper.ToDto(user) });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(UserMapper.ToDto(user));
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailTaken = await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId);
                if (emailTaken) return Conflict(new { message = "Email je već zauzet." });
                user.Email = dto.Email;
            }

            await _db.SaveChangesAsync();
            return Ok(UserMapper.ToDto(user));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Select(u => UserMapper.ToDto(u)).ToListAsync();
            return Ok(users);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(UserMapper.ToDto(user));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}