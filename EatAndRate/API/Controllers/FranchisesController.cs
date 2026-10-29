using API.Data;
using API.Models;
using EatAndRate.Shared.DTOs.Franchises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FranchisesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FranchisesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<FranchiseDto>>> GetAll()
        {
            var list = await ToDto(_db.Franchises)
                .OrderBy(f => f.Name)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FranchiseDto>> GetById(int id)
        {
            var franchise = await ToDto(_db.Franchises.Where(f => f.Id == id)).FirstOrDefaultAsync();

            if (franchise is null)
                return NotFound("Nincs ilyen franchise.");

            return Ok(franchise);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FranchiseDto>> Create(CreateFranchiseDto dto)
        {
            if (await _db.Franchises.AnyAsync(f => f.Name == dto.Name))
                return Conflict("Ilyen nevű franchise már létezik.");

            var franchise = new Franchise { Name = dto.Name };
            _db.Franchises.Add(franchise);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = franchise.Id },
                new FranchiseDto { Id = franchise.Id, Name = franchise.Name });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateFranchiseDto dto)
        {
            var franchise = await _db.Franchises.FindAsync(id);
            if (franchise is null)
                return NotFound("Nincs ilyen franchise.");

            if (await _db.Franchises.AnyAsync(f => f.Name == dto.Name && f.Id != id))
                return Conflict("Ilyen nevű franchise már létezik.");

            franchise.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var franchise = await _db.Franchises.FindAsync(id);
            if (franchise is null)
                return NotFound("Nincs ilyen franchise.");

            _db.Franchises.Remove(franchise);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<FranchiseDto> ToDto(IQueryable<Franchise> query) => query
            .Select(f => new FranchiseDto
            {
                Id = f.Id,
                Name = f.Name,
                RestaurantCount = f.Restaurants.Count,
                ReviewCount = f.Restaurants.SelectMany(r => r.Reviews).Count(),
                AvgOverall = f.Restaurants.SelectMany(r => r.Reviews)
                    .Average(x => (double?)(x.FoodRating + x.ServiceRating + x.AtmosphereRating)) / 3
            });
    }
}
