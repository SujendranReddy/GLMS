using GLMS.DTOs;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/service-requests")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            ICurrencyService currencyService,
            IServiceRequestService serviceRequestService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _currencyService = currencyService;
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            var serviceRequestDtos = serviceRequests
                .Select(MapToDto)
                .ToList();

            return Ok(serviceRequestDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceRequestDto>> GetServiceRequestById(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(serviceRequest));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest(
            [FromBody] CreateServiceRequestDto createServiceRequestDto)
        {
            var contract = await _contractRepository
                .GetByIdAsync(createServiceRequestDto.ContractId);

            if (contract == null)
            {
                return BadRequest(new
                {
                    message = "The selected contract could not be found."
                });
            }

            if (!_serviceRequestService.CanCreateRequest(contract))
            {
                return BadRequest(new
                {
                    message = "A service request cannot be created for an expired or on-hold contract."
                });
            }

            decimal costZar;

            try
            {
                costZar = await _currencyService
                    .ConvertUsdToZarAsync(createServiceRequestDto.CostUSD);
            }
            catch
            {
                return StatusCode(503, new
                {
                    message = "Unable to retrieve the exchange rate right now. Please try again."
                });
            }

            var serviceRequest = new ServiceRequest
            {
                ContractId = createServiceRequestDto.ContractId,
                Description = createServiceRequestDto.Description,
                CostUSD = createServiceRequestDto.CostUSD,
                CostZAR = costZar
            };

            var createdRequest = await _serviceRequestRepository
                .CreateAsync(serviceRequest);

            var savedRequest = await _serviceRequestRepository
                .GetByIdAsync(createdRequest.ServiceRequestId);

            var result = MapToDto(savedRequest ?? createdRequest);

            return CreatedAtAction(
                nameof(GetServiceRequestById),
                new { id = result.ServiceRequestId },
                result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ServiceRequestDto>> UpdateServiceRequest(
            int id,
            [FromBody] UpdateServiceRequestDto updateServiceRequestDto)
        {
            var existingRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (existingRequest == null)
            {
                return NotFound();
            }

            var contract = await _contractRepository
                .GetByIdAsync(updateServiceRequestDto.ContractId);

            if (contract == null)
            {
                return BadRequest(new
                {
                    message = "The selected contract could not be found."
                });
            }

            if (!_serviceRequestService.CanCreateRequest(contract))
            {
                return BadRequest(new
                {
                    message = "A service request cannot be assigned to an expired or on-hold contract."
                });
            }

            decimal costZar;

            try
            {
                costZar = await _currencyService
                    .ConvertUsdToZarAsync(updateServiceRequestDto.CostUSD);
            }
            catch
            {
                return StatusCode(503, new
                {
                    message = "Unable to retrieve the exchange rate right now. Please try again."
                });
            }

            var serviceRequest = new ServiceRequest
            {
                ServiceRequestId = id,
                ContractId = updateServiceRequestDto.ContractId,
                Description = updateServiceRequestDto.Description,
                CostUSD = updateServiceRequestDto.CostUSD,
                CostZAR = costZar
            };

            var updated = await _serviceRequestRepository.UpdateAsync(serviceRequest);

            if (!updated)
            {
                return NotFound();
            }

            var savedRequest = await _serviceRequestRepository.GetByIdAsync(id);

            return Ok(MapToDto(savedRequest!));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var deleted = await _serviceRequestRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static ServiceRequestDto MapToDto(ServiceRequest serviceRequest)
        {
            return new ServiceRequestDto
            {
                ServiceRequestId = serviceRequest.ServiceRequestId,
                ContractId = serviceRequest.ContractId,
                ContractDescription = serviceRequest.Contract?.Description ?? string.Empty,
                ClientName = serviceRequest.Contract?.Client?.Name ?? string.Empty,
                Description = serviceRequest.Description,
                CostUSD = serviceRequest.CostUSD,
                CostZAR = serviceRequest.CostZAR,
                CreatedDate = serviceRequest.CreatedDate
            };
        }
    }
}