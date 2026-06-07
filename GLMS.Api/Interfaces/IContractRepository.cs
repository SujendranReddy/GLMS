using GLMS.Enums;
using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllAsync(
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate);

        Task<Contract?> GetByIdAsync(int id);

        Task<Contract> CreateAsync(Contract contract);

        Task<bool> UpdateAsync(Contract contract);

        Task<bool> UpdateStatusAsync(int id, ContractStatus status);

        Task<bool> UpdateAgreementFilePathAsync(int id, string fileName);

        Task<bool> DeleteAsync(int id);
    }
}
