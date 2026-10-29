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
        [Required(ErrorMessage = "Felhasználónév vagy email kell")]
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "Jelszó kell")]
        public string Password { get; set; } = string.Empty;
    }
}
