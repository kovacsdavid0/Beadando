using API.Data;
using API.Models;
using EatAndRate.Shared.DTOs.Restaurants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RestaurantsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<RestaurantSummaryDto>>> GetAll([FromQuery] string? city, [FromQuery] int? franchiseId)
        {
            var query = _db.Restaurants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(r => r.City == city);

            if (franchiseId is not null)
                query = query.Where(r => r.FranchiseId == franchiseId);

            var list = await query
                .OrderBy(r => r.Name)
                .Select(r => new RestaurantSummaryDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    City = r.City,
                    Address = r.Address,
                    ReviewCount = r.Reviews.Count,
                    AvgFood = r.Reviews.Average(x => (double?)x.FoodRating),
                    AvgService = r.Reviews.Average(x => (double?)x.ServiceRating),
                    AvgAtmosphere = r.Reviews.Average(x => (double?)x.AtmosphereRating)
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDetailDto>> GetById(int id)
        {
            var restaurant = await FindDetail(id);

            if (restaurant is null)
                return NotFound("Nincs ilyen étterem.");

            return Ok(restaurant);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RestaurantDetailDto>> Create(CreateRestaurantDto dto)
        {
            if (dto.FranchiseId is not null && !await _db.Franchises.AnyAsync(f => f.Id == dto.FranchiseId))
                return BadRequest("Nincs ilyen franchise.");

            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                FranchiseId = dto.FranchiseId
            };

            _db.Restaurants.Add(restaurant);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, await FindDetail(restaurant.Id));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateRestaurantDto dto)
        {
            var restaurant = await _db.Restaurants.FindAsync(id);
            if (restaurant is null)
                return NotFound("Nincs ilyen étterem.");

            if (dto.FranchiseId is not null && !await _db.Franchises.AnyAsync(f => f.Id == dto.FranchiseId))
                return BadRequest("Nincs ilyen franchise.");

            restaurant.Name = dto.Name;
            restaurant.Address = dto.Address;
            restaurant.City = dto.City;
            restaurant.FranchiseId = dto.FranchiseId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _db.Restaurants.FindAsync(id);
            if (restaurant is null)
                return NotFound("Nincs ilyen étterem.");

            _db.Restaurants.Remove(restaurant);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private Task<RestaurantDetailDto?> FindDetail(int id) => _db.Restaurants
            .Where(r => r.Id == id)
            .Select(r => new RestaurantDetailDto
            {
                Id = r.Id,
                Name = r.Name,
                Address = r.Address,
                City = r.City,
                FranchiseId = r.FranchiseId,
                FranchiseName = r.Franchise != null ? r.Franchise.Name : null,
                ReviewCount = r.Reviews.Count,
                AvgFood = r.Reviews.Average(x => (double?)x.FoodRating),
                AvgService = r.Reviews.Average(x => (double?)x.ServiceRating),
                AvgAtmosphere = r.Reviews.Average(x => (double?)x.AtmosphereRating)
            })
            .FirstOrDefaultAsync();
    }
}
