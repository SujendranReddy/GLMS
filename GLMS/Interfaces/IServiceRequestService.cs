using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IServiceRequestService
    {
        bool CanCreateRequest(Contract contract);
    }
}