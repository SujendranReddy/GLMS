using GLMS.Enums;
using GLMS.Models;
using GLMS.Services;
using Xunit;

namespace GLMS.Tests
{
    public class ServiceRequestServiceTests
    {
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            _service = new ServiceRequestService();
        }

        [Fact]
        public void CanCreateRequest_ReturnsTrue_ForActiveContract()
        {
            var contract = new Contract { Status = ContractStatus.Active };

            var result = _service.CanCreateRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void CanCreateRequest_ReturnsTrue_ForDraftContract()
        {
            var contract = new Contract { Status = ContractStatus.Draft };

            var result = _service.CanCreateRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void CanCreateRequest_ReturnsFalse_ForExpiredContract()
        {
            var contract = new Contract { Status = ContractStatus.Expired };

            var result = _service.CanCreateRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateRequest_ReturnsFalse_ForOnHoldContract()
        {
            var contract = new Contract { Status = ContractStatus.OnHold };

            var result = _service.CanCreateRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateRequest_ReturnsFalse_ForNullContract()
        {
            var result = _service.CanCreateRequest(null);

            Assert.False(result);
        }
    }
}