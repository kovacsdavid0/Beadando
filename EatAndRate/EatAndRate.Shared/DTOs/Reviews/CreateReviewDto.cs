using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EatAndRate.Shared.DTOs.Reviews
{
    public class CreateReviewDto
    {
        [Range(1, 5, ErrorMessage = "Az étel értékelése 1 és 5 között lehet.")]
        public int FoodRating { get; set; }

        [Range(1, 5, ErrorMessage = "A kiszolgálás értékelése 1 és 5 között lehet.")]
        public int ServiceRating { get; set; }

        [Range(1, 5, ErrorMessage = "A hangulat értékelése 1 és 5 között lehet.")]
        public int AtmosphereRating { get; set; }

        [MaxLength(1000, ErrorMessage = "A megjegyzés legfeljebb 1000 karakter lehet.")]
        public string? Comment { get; set; }

    }
}
