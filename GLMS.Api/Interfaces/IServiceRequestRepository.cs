using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<List<ServiceRequest>> GetAllAsync();

        Task<ServiceRequest?> GetByIdAsync(int id);

        Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest);

        Task<bool> UpdateAsync(ServiceRequest serviceRequest);

        Task<bool> DeleteAsync(int id);
    }
}
