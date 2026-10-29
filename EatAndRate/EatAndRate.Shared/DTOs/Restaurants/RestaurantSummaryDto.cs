using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace EatAndRate.Shared.DTOs.Restaurants
{
    public class RestaurantSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Address { get; set; }

        public int ReviewCount { get; set; }
        public double? AvgFood { get; set; }
        public double? AvgService { get; set; }
        public double? AvgAtmosphere { get; set; }

        public double? AvgOverall => (AvgFood + AvgService + AvgAtmosphere) / 3;
    }
}
