using GLMS.Models;

namespace GLMS.Services.Api
{
    public interface IServiceRequestApiService
    {
        Task<List<ServiceRequest>> GetAllAsync();

        Task<ServiceRequest?> GetByIdAsync(int id);

        Task<ServiceRequest?> CreateAsync(ServiceRequest serviceRequest);

        Task<bool> UpdateAsync(int id, ServiceRequest serviceRequest);

        Task<bool> DeleteAsync(int id);
    }
}