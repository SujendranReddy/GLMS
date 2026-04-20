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
                CreatedDate = DateTime.Now,
                Status = Enums.ContractStatus.Draft
            };
        }
    }
}