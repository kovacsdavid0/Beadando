using EatAndRate.Shared.DTOs.Auth;

namespace Frontend.Services
{
    public static class Session
    {
        public static string? Token { get; private set; }
        public static UserDto? User { get; private set; }

        public static bool IsLoggedIn => Token is not null;

        public static void Start(AuthResponseDto auth)
        {
            Token = auth.Token;
            User = auth.User;
            ApiClient.SetToken(auth.Token);
        }

        public static void End()
        {
            Token = null;
            User = null;
            ApiClient.SetToken(null);
        }
    }
}
