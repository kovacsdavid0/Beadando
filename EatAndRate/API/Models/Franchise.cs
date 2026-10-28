namespace API.Models
{
    public class Franchise
    {
        public string Name { get; set; } = null!;
        public string FranchiseId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
