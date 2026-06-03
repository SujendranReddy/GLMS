using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IServiceRequestService
    {
        // This is sued to determine whether a service request can be created for a contract
        bool CanCreateRequest(Contract contract);
    }
}