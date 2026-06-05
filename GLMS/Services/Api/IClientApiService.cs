using GLMS.Models;

namespace GLMS.Services.Api
{
    public interface IClientApiService
    {
        Task<List<Client>> GetAllAsync();

        Task<Client?> GetByIdAsync(int id);

        Task<Client?> CreateAsync(Client client);

        Task<bool> UpdateAsync(int id, Client client);

        Task<bool> DeleteAsync(int id);
    }
}