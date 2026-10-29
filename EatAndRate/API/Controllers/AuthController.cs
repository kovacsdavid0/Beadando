using API.Data;
using API.Models;
using EatAndRate.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return Conflict("A felhasználónév vagy az e-mail már foglalt.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BC.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Sikeres regisztráció.",
                user = new UserDto { Id = user.Id, Username = user.Username }
            });
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
    u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user is null || !BC.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Hibás felhasználónév vagy jelszó.");

            return Ok(new
            {
                message = "Sikeres bejelentkezés.",
                user = new UserDto { Id = user.Id, Username = user.Username }
            });
        }
    }
}