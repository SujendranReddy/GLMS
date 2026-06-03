using GLMS.Data;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c!.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c!.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
        {
            serviceRequest.CreatedDate = DateTime.Now;

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<bool> UpdateAsync(ServiceRequest serviceRequest)
        {
            var existingRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(s => s.ServiceRequestId == serviceRequest.ServiceRequestId);

            if (existingRequest == null)
            {
                return false;
            }

            existingRequest.ContractId = serviceRequest.ContractId;
            existingRequest.Description = serviceRequest.Description;
            existingRequest.CostUSD = serviceRequest.CostUSD;
            existingRequest.CostZAR = serviceRequest.CostZAR;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return false;
            }

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
