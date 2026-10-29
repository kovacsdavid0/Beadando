using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EatAndRate.Shared.DTOs.Franchises
{
    public class FranchiseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RestaurantCount { get; set; }
        public int ReviewCount { get; set; }
        public double? AvgOverall { get; set; }
    }
}
