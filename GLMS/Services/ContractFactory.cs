using GLMS.Interfaces;
using GLMS.Models;

namespace GLMS.Services
{
    public class ContractFactory : IContractFactory
    {
        public Contract Create()
        {
            return new Contract
            {
                // This creates a contract with these default values to make the creation standardized, following a1 design
                CreatedDate = DateTime.Now,
                Status = Enums.ContractStatus.Draft
            };
        }
    }
}