using GLMS.Interfaces;
using System.Text.Json;

namespace GLMS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var zarRate = document
                .RootElement
                .GetProperty("rates")
                .GetProperty("ZAR")
                .GetDecimal();

            return zarRate;
        }

        public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
        {
            var rate = await GetUsdToZarRateAsync();

            return Math.Round(usdAmount * rate, 2);
        }
    }
}