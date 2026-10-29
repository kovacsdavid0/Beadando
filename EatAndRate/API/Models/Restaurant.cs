namespace API.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? City { get; set; } = null!;
        public int? FranchiseId { get; set; }
        public Franchise? Franchise { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
