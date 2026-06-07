using GLMS.Data;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == client.ClientId);

            if (existingClient == null)
            {
                return false;
            }

            existingClient.Name = client.Name;
            existingClient.Email = client.Email;
            existingClient.PhoneNumber = client.PhoneNumber;
            existingClient.Region = client.Region;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return false;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}