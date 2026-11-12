using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EatAndRate.Shared.DTOs.Franchises
{
    public class CreateFranchiseDto
    {
        [Required(ErrorMessage = "Név kötelező")]
        [MaxLength(100, ErrorMessage = "A név max 100 karakter lehet")]
        public string Name { get; set; } = string.Empty;
    }
}
