using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EatAndRate.Shared.DTOs.Franchises
{
    public class CreateFranchiseDto
    {
        [Required(ErrorMessage = "Név kell")]
        [MaxLength(100, ErrorMessage = "Név max 100 karakter")]
        public string Name { get; set; } = string.Empty;
    }
}
