using GLMS.Models;
using System.Net.Http.Json;

namespace GLMS.Services.Api
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            var apiRequests = await _httpClient
                .GetFromJsonAsync<List<ApiServiceRequestDto>>("api/service-requests");

            return apiRequests?
                .Select(MapToServiceRequest)
                .ToList() ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            var apiRequest = await _httpClient
                .GetFromJsonAsync<ApiServiceRequestDto>($"api/service-requests/{id}");

            return apiRequest == null ? null : MapToServiceRequest(apiRequest);
        }

        public async Task<ServiceRequest?> CreateAsync(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/service-requests",
                new
                {
                    serviceRequest.ContractId,
                    serviceRequest.Description,
                    serviceRequest.CostUSD
                });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var apiRequest = await response.Content.ReadFromJsonAsync<ApiServiceRequestDto>();

            return apiRequest == null ? null : MapToServiceRequest(apiRequest);
        }

        public async Task<bool> UpdateAsync(int id, ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/service-requests/{id}",
                new
                {
                    serviceRequest.ContractId,
                    serviceRequest.Description,
                    serviceRequest.CostUSD
                });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/service-requests/{id}");

            return response.IsSuccessStatusCode;
        }

        private static ServiceRequest MapToServiceRequest(ApiServiceRequestDto apiRequest)
        {
            return new ServiceRequest
            {
                ServiceRequestId = apiRequest.ServiceRequestId,
                ContractId = apiRequest.ContractId,
                Contract = new Contract
                {
                    ContractId = apiRequest.ContractId,
                    Description = apiRequest.ContractDescription,
                    Client = new Client
                    {
                        Name = apiRequest.ClientName
                    }
                },
                Description = apiRequest.Description,
                CostUSD = apiRequest.CostUSD,
                CostZAR = apiRequest.CostZAR,
                CreatedDate = apiRequest.CreatedDate
            };
        }

        private class ApiServiceRequestDto
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