using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EatAndRate.Shared.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username kell")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username 3 es 50 karakter")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email kell")]
        [EmailAddress(ErrorMessage = "nem jo")]
        [StringLength(100, ErrorMessage = "Email max 100 karakter")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Jelszó kell")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Jelszó 6 es 100 karakter")]
        public string Password { get; set; } = string.Empty;
    }
}
