namespace API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ServiceRating { get; set; }
        public int FoodRating { get; set; }
        public int AtmosphereRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
    }
}
