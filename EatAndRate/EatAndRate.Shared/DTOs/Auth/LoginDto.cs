using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EatAndRate.Shared.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email kell")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Jelszó kell")]
        public string Password { get; set; } = string.Empty;
    }
}
