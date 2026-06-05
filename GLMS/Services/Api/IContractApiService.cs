using GLMS.Enums;
using GLMS.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace GLMS.Services.Api
{
    public interface IContractApiService
    {
        Task<List<Contract>> GetAllAsync(
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate);

        Task<Contract?> GetByIdAsync(int id);

        Task<Contract?> CreateAsync(Contract contract);

        Task<bool> UpdateAsync(int id, Contract contract);

        Task<bool> UpdateStatusAsync(int id, ContractStatus status);

        Task<bool> UploadAgreementAsync(int id, IFormFile pdfFile);

        Task<HttpResponseMessage> DownloadAgreementAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}