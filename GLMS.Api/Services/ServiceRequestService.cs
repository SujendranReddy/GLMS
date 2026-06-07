using GLMS.Enums;
using GLMS.Interfaces;
using GLMS.Models;

namespace GLMS.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public bool CanCreateRequest(Contract contract)
        {
            if (contract == null)
            {
                return false;
            }

            // Blocks service requests for contracts that should not accept work.
            return contract.Status != ContractStatus.Expired &&
                   contract.Status != ContractStatus.OnHold;
        }
    }
}