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
    //Calls the API to get the latest USD to ZAR rate
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            //Parse the json response and get the ZAR exchange rate
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
            //Convert the amount and round off to 2 decimals
            return Math.Round(usdAmount * rate, 2);
        }
    }
}