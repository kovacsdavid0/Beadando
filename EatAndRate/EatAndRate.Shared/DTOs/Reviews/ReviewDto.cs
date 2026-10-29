using System;
using System.Collections.Generic;
using System.Text;

namespace EatAndRate.Shared.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int FoodRating { get; set; }
        public int ServiceRating { get; set; }
        public int AtmosphereRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
    }
}
