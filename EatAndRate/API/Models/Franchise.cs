namespace API.Models
{
    public class Franchise
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
