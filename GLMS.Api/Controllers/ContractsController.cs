using GLMS.DTOs;
using GLMS.Enums;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractsController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        // GET: api/contracts
        // Supports filtering by status and created date range.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContracts(
            [FromQuery] ContractStatus? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var contracts = await _contractRepository
                .GetAllAsync(status, startDate, endDate);

            var contractDtos = contracts
                .Select(MapToDto)
                .ToList();

            return Ok(contractDtos);
        }

        // GET: api/contracts/5
        // Needed by the MVC frontend for contract details.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractDto>> GetContractById(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(contract));
        }

        // POST: api/contracts
        [HttpPost]
        public async Task<ActionResult<ContractDto>> CreateContract(
            [FromBody] CreateContractDto createContractDto)
        {
            var contract = new Contract
            {
                ClientId = createContractDto.ClientId,
                Description = createContractDto.Description,
                Cost = createContractDto.Cost,
                Status = createContractDto.Status,
                SignedAgreementFilePath = createContractDto.SignedAgreementFilePath
            };

            var createdContract = await _contractRepository.CreateAsync(contract);

            var savedContract = await _contractRepository
                .GetByIdAsync(createdContract.ContractId);

            var result = MapToDto(savedContract ?? createdContract);

            return CreatedAtAction(
                nameof(GetContractById),
                new { id = result.ContractId },
                result);
        }

        // PATCH: api/contracts/5/status
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ContractDto>> UpdateContractStatus(
            int id,
            [FromBody] UpdateContractStatusDto updateStatusDto)
        {
            var updated = await _contractRepository
                .UpdateStatusAsync(id, updateStatusDto.Status);

            if (!updated)
            {
                return NotFound();
            }

            var contract = await _contractRepository.GetByIdAsync(id);

            return Ok(MapToDto(contract!));
        }

        private static ContractDto MapToDto(Contract contract)
        {
            return new ContractDto
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                ClientName = contract.Client?.Name ?? string.Empty,
                Description = contract.Description,
                Cost = contract.Cost,
                Status = contract.Status.ToString(),
                SignedAgreementFilePath = contract.SignedAgreementFilePath,
                CreatedDate = contract.CreatedDate
            };
        }
    }
}