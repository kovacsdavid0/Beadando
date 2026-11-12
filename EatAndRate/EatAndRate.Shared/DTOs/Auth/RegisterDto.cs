using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EatAndRate.Shared.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username kötelező")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "A username 3 és 50 karakter között kell legyen")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email kötelező")]
        [EmailAddress(ErrorMessage = "A megadott email cím helytelen")]
        [StringLength(100, ErrorMessage = "Email max 100 karakter")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Jelszó kötelező")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A jelszó 6 és 100 karakter között kell legyen")]
        public string Password { get; set; } = string.Empty;
    }
}
