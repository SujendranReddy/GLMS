using System.Net.Http.Json;

namespace GLMS.Services.Api
{
    public class ApiTokenService : IApiTokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private string? _cachedToken;
        private DateTime _tokenExpiryTime = DateTime.MinValue;

        public ApiTokenService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_cachedToken) &&
                DateTime.UtcNow < _tokenExpiryTime)
            {
                return _cachedToken;
            }

            var username = _configuration["ApiAuth:Username"];
            var password = _configuration["ApiAuth:Password"];

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var client = _httpClientFactory.CreateClient("GLMSAuthClient");

            var response = await client.PostAsJsonAsync(
                "api/auth/login",
                new LoginRequest
                {
                    Username = username,
                    Password = password
                });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenResponse = await response.Content
                .ReadFromJsonAsync<LoginResponse>();

            if (string.IsNullOrWhiteSpace(tokenResponse?.Token))
            {
                return null;
            }

            _cachedToken = tokenResponse.Token;
            _tokenExpiryTime = DateTime.UtcNow.AddMinutes(110);

            return _cachedToken;
        }

        private class LoginRequest
        {
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}