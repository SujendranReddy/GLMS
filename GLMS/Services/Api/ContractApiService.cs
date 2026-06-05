using GLMS.Enums;
using GLMS.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace GLMS.Services.Api
{
    public class ContractApiService : IContractApiService
    {
        private readonly HttpClient _httpClient;

        public ContractApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Contract>> GetAllAsync(
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = new List<string>();

            if (status.HasValue)
            {
                query.Add($"status={(int)status.Value}");
            }

            if (startDate.HasValue)
            {
                query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            }

            if (endDate.HasValue)
            {
                query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            }

            var url = "api/contracts";

            if (query.Any())
            {
                url += "?" + string.Join("&", query);
            }

            var apiContracts = await _httpClient
                .GetFromJsonAsync<List<ApiContractDto>>(url);

            return apiContracts?
                .Select(MapToContract)
                .ToList() ?? new List<Contract>();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            var apiContract = await _httpClient
                .GetFromJsonAsync<ApiContractDto>($"api/contracts/{id}");

            return apiContract == null ? null : MapToContract(apiContract);
        }

        public async Task<Contract?> CreateAsync(Contract contract)
        {
            var response = await _httpClient.PostAsJsonAsync("api/contracts", contract);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var apiContract = await response.Content.ReadFromJsonAsync<ApiContractDto>();

            return apiContract == null ? null : MapToContract(apiContract);
        }

        public async Task<bool> UpdateAsync(int id, Contract contract)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/contracts/{id}", contract);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStatusAsync(int id, ContractStatus status)
        {
            var response = await _httpClient.PatchAsJsonAsync(
                $"api/contracts/{id}/status",
                new { status });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadAgreementAsync(int id, IFormFile pdfFile)
        {
            using var content = new MultipartFormDataContent();

            using var stream = pdfFile.OpenReadStream();

            var fileContent = new StreamContent(stream);

            if (!string.IsNullOrWhiteSpace(pdfFile.ContentType))
            {
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(pdfFile.ContentType);
            }

            content.Add(fileContent, "pdfFile", pdfFile.FileName);

            var response = await _httpClient.PostAsync(
                $"api/contracts/{id}/agreement",
                content);

            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> DownloadAgreementAsync(int id)
        {
            return await _httpClient.GetAsync($"api/contracts/{id}/agreement");
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");

            return response.IsSuccessStatusCode;
        }

        private static Contract MapToContract(ApiContractDto apiContract)
        {
            Enum.TryParse(
                apiContract.Status,
                ignoreCase: true,
                out ContractStatus contractStatus);

            return new Contract
            {
                ContractId = apiContract.ContractId,
                ClientId = apiContract.ClientId,
                Client = new Client
                {
                    ClientId = apiContract.ClientId,
                    Name = apiContract.ClientName
                },
                Description = apiContract.Description,
                Cost = apiContract.Cost,
                Status = contractStatus,
                SignedAgreementFilePath = apiContract.SignedAgreementFilePath,
                CreatedDate = apiContract.CreatedDate
            };
        }

        private class ApiContractDto
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
    }
}