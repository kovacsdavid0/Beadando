using API.Data;
using API.Models;
using API.Services;
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
        private readonly TokenService _tokens;

        public AuthController(AppDbContext db, TokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            // A felhasználónév nem lehet e-mail formátumú, különben a bejelentkezésnél összeakadhat más e-mail címével
            if (dto.Username.Contains('@'))
                return BadRequest("A felhasználónév nem tartalmazhat @ karaktert.");

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return Conflict("A felhasználónév vagy az e-mail már foglalt.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BC.HashPassword(dto.Password)
            };

            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Két egyidejű regisztrációnál az egyedi index dobja el a másodikat
                return Conflict("A felhasználónév vagy az e-mail már foglalt.");
            }

            return Ok(CreateResponse(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user is null || !BC.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Hibás felhasználónév vagy jelszó.");

            return Ok(CreateResponse(user));
        }

        private AuthResponseDto CreateResponse(User user) => new()
        {
            Token = _tokens.CreateToken(user),
            User = new UserDto { Id = user.Id, Username = user.Username }
        };
    }
}
