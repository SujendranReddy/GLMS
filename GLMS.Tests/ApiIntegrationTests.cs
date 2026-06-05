using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests
{
    public class ApiIntegrationTests
    {
        private const string BaseUrl = "https://localhost:7225/";

        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        private static async Task<string> GetJwtTokenAsync(HttpClient client)
        {
            var response = await client.PostAsJsonAsync("api/auth/login", new
            {
                username = "admin",
                password = "Password123!"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var tokenResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.Token));

            return tokenResponse.Token;
        }

        [Fact]
        public async Task GetClients_WithoutToken_ReturnsUnauthorized()
        {
            using var client = CreateClient();

            var response = await client.GetAsync("api/clients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            using var client = CreateClient();

            var token = await GetJwtTokenAsync(client);

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task GetContracts_WithToken_ReturnsOkAndJson()
        {
            using var client = CreateClient();

            var token = await GetJwtTokenAsync(client);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractResponse>>();

            Assert.NotNull(contracts);
        }

        [Fact]
        public async Task GetClients_WithToken_ReturnsOkAndJson()
        {
            using var client = CreateClient();

            var token = await GetJwtTokenAsync(client);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var clients = await response.Content.ReadFromJsonAsync<List<ClientResponse>>();

            Assert.NotNull(clients);
        }

        [Fact]
        public async Task GetServiceRequests_WithToken_ReturnsOkAndJson()
        {
            using var client = CreateClient();

            var token = await GetJwtTokenAsync(client);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/service-requests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var serviceRequests = await response.Content.ReadFromJsonAsync<List<ServiceRequestResponse>>();

            Assert.NotNull(serviceRequests);
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private class ClientResponse
        {
            public int ClientId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
        }

        private class ContractResponse
        {
            public int ContractId { get; set; }
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? SignedAgreementFilePath { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        private class ServiceRequestResponse
        {
            public int ServiceRequestId { get; set; }
            public int ContractId { get; set; }
            public string ContractDescription { get; set; } = string.Empty;
            public string ClientName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal CostUSD { get; set; }
            public decimal CostZAR { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}