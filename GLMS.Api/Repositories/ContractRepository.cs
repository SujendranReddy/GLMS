using GLMS.Data;
using GLMS.Enums;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllAsync(
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (status.HasValue)
            {
                contracts = contracts.Where(c => c.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.CreatedDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var endDateExclusive = endDate.Value.Date.AddDays(1);

                contracts = contracts.Where(c => c.CreatedDate < endDateExclusive);
            }

            return await contracts.ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            contract.CreatedDate = DateTime.Now;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<bool> UpdateAsync(Contract contract)
        {
            var existingContract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId);

            if (existingContract == null)
            {
                return false;
            }

            existingContract.ClientId = contract.ClientId;
            existingContract.Description = contract.Description;
            existingContract.Cost = contract.Cost;
            existingContract.Status = contract.Status;

            if (!string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                existingContract.SignedAgreementFilePath = contract.SignedAgreementFilePath;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, ContractStatus status)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            contract.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAgreementFilePathAsync(int id, string fileName)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            contract.SignedAgreementFilePath = fileName;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}