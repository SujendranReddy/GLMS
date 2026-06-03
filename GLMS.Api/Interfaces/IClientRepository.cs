using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllAsync();

        Task<Client?> GetByIdAsync(int id);

        Task<Client> CreateAsync(Client client);

        Task<bool> UpdateAsync(Client client);

        Task<bool> DeleteAsync(int id);
    }
}