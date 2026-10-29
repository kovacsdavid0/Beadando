using System;
using System.Collections.Generic;
using System.Text;

namespace EatAndRate.Shared.DTOs.Restaurants
{
    public class RestaurantDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }

        public int? FranchiseId { get; set; }
        public string? FranchiseName { get; set; }

        public int ReviewCount { get; set; }
        public double? AvgFood { get; set; }
        public double? AvgService { get; set; }
        public double? AvgAtmosphere { get; set; }

        public double? AvgOverall => (AvgFood + AvgService + AvgAtmosphere) / 3;
    }
}
