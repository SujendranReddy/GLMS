using GLMS.DTOs;
using GLMS.Enums;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/contracts")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IFileService _fileService;
        private readonly IContractFactory _contractFactory;
        private readonly ISubject _subject;
        private readonly IObserver _observer;

        public ContractsController(
            IContractRepository contractRepository,
            IFileService fileService,
            IContractFactory contractFactory,
            ISubject subject,
            IObserver observer)
        {
            _contractRepository = contractRepository;
            _fileService = fileService;
            _contractFactory = contractFactory;
            _subject = subject;
            _observer = observer;
        }

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

        [HttpPost]
        public async Task<ActionResult<ContractDto>> CreateContract(
            [FromBody] CreateContractDto createContractDto)
        {
            var contract = _contractFactory.Create();

            contract.ClientId = createContractDto.ClientId;
            contract.Description = createContractDto.Description;
            contract.Cost = createContractDto.Cost;
            contract.Status = createContractDto.Status;
            contract.SignedAgreementFilePath = createContractDto.SignedAgreementFilePath;

            var createdContract = await _contractRepository.CreateAsync(contract);

            // Notifies the audit logger when a contract is created.
            _subject.Attach(_observer);
            _subject.Notify($"Contract '{createdContract.ContractId}' was created with status '{createdContract.Status}'.");

            var savedContract = await _contractRepository
                .GetByIdAsync(createdContract.ContractId);

            var result = MapToDto(savedContract ?? createdContract);

            return CreatedAtAction(
                nameof(GetContractById),
                new { id = result.ContractId },
                result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractDto>> UpdateContract(
            int id,
            [FromBody] UpdateContractDto updateContractDto)
        {
            var existingContract = await _contractRepository.GetByIdAsync(id);

            if (existingContract == null)
            {
                return NotFound();
            }

            var contract = new Contract
            {
                ContractId = id,
                ClientId = updateContractDto.ClientId,
                Description = updateContractDto.Description,
                Cost = updateContractDto.Cost,
                Status = updateContractDto.Status,
                SignedAgreementFilePath = updateContractDto.SignedAgreementFilePath
            };

            var updated = await _contractRepository.UpdateAsync(contract);

            if (!updated)
            {
                return NotFound();
            }

            var savedContract = await _contractRepository.GetByIdAsync(id);

            return Ok(MapToDto(savedContract!));
        }

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

        [HttpPost("{id:int}/agreement")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ContractDto>> UploadAgreement(
            int id,
            IFormFile pdfFile)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return NotFound(new
                {
                    message = "The selected contract could not be found."
                });
            }

            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Please select a PDF agreement to upload."
                });
            }

            if (!_fileService.IsPdf(pdfFile))
            {
                return BadRequest(new
                {
                    message = "Only PDF files are allowed."
                });
            }

            var savedFileName = await _fileService.SavePdfAsync(pdfFile);

            if (string.IsNullOrWhiteSpace(savedFileName))
            {
                return BadRequest(new
                {
                    message = "The PDF agreement could not be saved."
                });
            }

            await _contractRepository.UpdateAgreementFilePathAsync(id, savedFileName);

            var updatedContract = await _contractRepository.GetByIdAsync(id);

            return Ok(MapToDto(updatedContract!));
        }

        [HttpGet("{id:int}/agreement")]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return NotFound(new
                {
                    message = "The selected contract could not be found."
                });
            }

            var filePath = _fileService.GetPdfPath(contract.SignedAgreementFilePath);

            if (filePath == null)
            {
                return NotFound(new
                {
                    message = "No signed PDF agreement was found for this contract."
                });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(
                fileBytes,
                "application/pdf",
                $"Contract-{contract.ContractId}-Agreement.pdf");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            var deleted = await _contractRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            // Keeps stored PDF files in sync with deleted contracts.
            if (!string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                _fileService.DeletePdf(contract.SignedAgreementFilePath);
            }

            return NoContent();
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