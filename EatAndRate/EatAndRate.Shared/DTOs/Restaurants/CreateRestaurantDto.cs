using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EatAndRate.Shared.DTOs.Restaurants
{
    public class CreateRestaurantDto
    {
        [Required(ErrorMessage = "A név kötelező.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }
        public int? FranchiseId { get; set; }
    }
}
