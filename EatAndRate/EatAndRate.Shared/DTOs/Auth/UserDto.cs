using System;
using System.Collections.Generic;
using System.Text;

namespace EatAndRate.Shared.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
