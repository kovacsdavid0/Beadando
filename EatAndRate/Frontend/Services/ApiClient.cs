using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EatAndRate.Shared.DTOs.Auth;

namespace Frontend.Services
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }

    public static class ApiClient
    {
        //"http" profil: 5029, docker-compose: 8080
        private const string BaseUrl = "http://localhost:8080/";

        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };

        public static void SetToken(string? token)
        {
            _http.DefaultRequestHeaders.Authorization =
                token is null ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        public static Task<AuthResponseDto> RegisterAsync(RegisterDto dto) =>
            PostAsync<AuthResponseDto>("api/auth/register", dto);

        public static Task<AuthResponseDto> LoginAsync(LoginDto dto) =>
            PostAsync<AuthResponseDto>("api/auth/login", dto);

        private static async Task<T> PostAsync<T>(string url, object body)
        {
            HttpResponseMessage response;
            try
            {
                response = await _http.PostAsJsonAsync(url, body);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new ApiException("Nem sikerült kapcsolódni a szerverhez. Fut az API?");
            }

            if (!response.IsSuccessStatusCode)
                throw new ApiException(await ReadErrorAsync(response));

            return await response.Content.ReadFromJsonAsync<T>()
                ?? throw new ApiException("Üres válasz érkezett a szervertől.");
        }

        // Az API vagy sima szöveget ad vissza (pl. Conflict("...")),
        // vagy ValidationProblemDetails JSON-t az "errors" mezővel.
        private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            var text = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(text))
                return $"Hiba történt ({(int)response.StatusCode}).";

            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.String)
                    return root.GetString()!;

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = errors.EnumerateObject()
                        .SelectMany(p => p.Value.EnumerateArray().Select(m => m.GetString()))
                        .Where(m => !string.IsNullOrEmpty(m));
                    return string.Join("\n", messages);
                }

                if (root.TryGetProperty("title", out var title))
                    return title.GetString() ?? text;
            }
            catch (JsonException)
            {
            }

            return text;
        }
    }
}
