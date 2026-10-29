using System;
using System.Collections.Generic;
using System.Text;

namespace EatAndRate.Shared.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
    }
}
