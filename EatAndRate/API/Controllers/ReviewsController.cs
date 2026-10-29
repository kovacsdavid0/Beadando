using API.Data;
using API.Models;
using EatAndRate.Shared.DTOs.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReviewsController(AppDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("restaurants/{restaurantId}/reviews")]
        public async Task<ActionResult<List<ReviewDto>>> GetForRestaurant(int restaurantId)
        {
            if (!await _db.Restaurants.AnyAsync(r => r.Id == restaurantId))
                return NotFound("Nincs ilyen étterem.");

            var list = await _db.Reviews
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(ToDto)
                .ToListAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("reviews/mine")]
        public async Task<ActionResult<List<ReviewDto>>> GetMine()
        {
            var userId = CurrentUserId;

            var list = await _db.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(ToDto)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("reviews/{id}")]
        public async Task<ActionResult<ReviewDto>> GetById(int id)
        {
            var review = await _db.Reviews.Where(r => r.Id == id).Select(ToDto).FirstOrDefaultAsync();

            if (review is null)
                return NotFound("Nincs ilyen értékelés.");

            return Ok(review);
        }

        [Authorize]
        [HttpPost("restaurants/{restaurantId}/reviews")]
        public async Task<ActionResult<ReviewDto>> Create(int restaurantId, CreateReviewDto dto)
        {
            var userId = CurrentUserId;

            if (!await _db.Restaurants.AnyAsync(r => r.Id == restaurantId))
                return NotFound("Nincs ilyen étterem.");

            if (await _db.Reviews.AnyAsync(r => r.RestaurantId == restaurantId && r.UserId == userId))
                return Conflict("Ezt az éttermet már értékelted.");

            var review = new Review
            {
                RestaurantId = restaurantId,
                UserId = userId,
                FoodRating = dto.FoodRating,
                ServiceRating = dto.ServiceRating,
                AtmosphereRating = dto.AtmosphereRating,
                Comment = dto.Comment
            };

            _db.Reviews.Add(review);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("Ezt az éttermet már értékelted.");
            }

            var created = await _db.Reviews.Where(r => r.Id == review.Id).Select(ToDto).FirstAsync();
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, created);
        }

        [Authorize]
        [HttpPut("reviews/{id}")]
        public async Task<IActionResult> Update(int id, CreateReviewDto dto)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review is null)
                return NotFound("Nincs ilyen értékelés.");

            if (review.UserId != CurrentUserId)
                return Forbid();

            review.FoodRating = dto.FoodRating;
            review.ServiceRating = dto.ServiceRating;
            review.AtmosphereRating = dto.AtmosphereRating;
            review.Comment = dto.Comment;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("reviews/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review is null)
                return NotFound("Nincs ilyen értékelés.");

            if (review.UserId != CurrentUserId)
                return Forbid();

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static readonly Expression<Func<Review, ReviewDto>> ToDto = r => new ReviewDto
        {
            Id = r.Id,
            FoodRating = r.FoodRating,
            ServiceRating = r.ServiceRating,
            AtmosphereRating = r.AtmosphereRating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UserId = r.UserId,
            Username = r.User.Username,
            RestaurantId = r.RestaurantId,
            RestaurantName = r.Restaurant.Name
        };
    }
}
