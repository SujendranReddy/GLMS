using GLMS.Enums;
using GLMS.Models;
using GLMS.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractApiService _contractApiService;
        private readonly IClientApiService _clientApiService;

        public ContractsController(
            IContractApiService contractApiService,
            IClientApiService clientApiService)
        {
            _contractApiService = contractApiService;
            _clientApiService = clientApiService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = await _contractApiService
                .GetAllAsync(status, startDate, endDate);

            ViewBag.SelectedStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            await PopulateClientsAsync();

            var contract = new Contract
            {
                CreatedDate = DateTime.Now,
                Status = ContractStatus.Draft
            };

            return View(contract);
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? pdfFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            var createdContract = await _contractApiService.CreateAsync(contract);

            if (createdContract == null)
            {
                ModelState.AddModelError("", "Unable to create the contract through the API. Please try again.");
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            if (pdfFile != null && pdfFile.Length > 0)
            {
                var uploaded = await _contractApiService
                    .UploadAgreementAsync(createdContract.ContractId, pdfFile);

                if (!uploaded)
                {
                    TempData["Error"] = "The contract was created, but the PDF agreement could not be uploaded.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            await PopulateClientsAsync(contract.ClientId);

            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? pdfFile)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            var existingContract = await _contractApiService.GetByIdAsync(id);

            if (existingContract == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            contract.SignedAgreementFilePath = existingContract.SignedAgreementFilePath;
            contract.CreatedDate = existingContract.CreatedDate;

            var updated = await _contractApiService.UpdateAsync(id, contract);

            if (!updated)
            {
                ModelState.AddModelError("", "Unable to update the contract through the API. Please try again.");
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            if (pdfFile != null && pdfFile.Length > 0)
            {
                var uploaded = await _contractApiService
                    .UploadAgreementAsync(id, pdfFile);

                if (!uploaded)
                {
                    TempData["Error"] = "The contract was updated, but the PDF agreement could not be uploaded.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _contractApiService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // Keeps old view links working if they pass fileName, but downloads safely through the API by contract ID
        public async Task<IActionResult> Download(string? fileName, int? id)
        {
            int? contractId = id;

            if (contractId == null && !string.IsNullOrWhiteSpace(fileName))
            {
                var contracts = await _contractApiService.GetAllAsync(null, null, null);

                contractId = contracts
                    .FirstOrDefault(c => c.SignedAgreementFilePath == fileName)
                    ?.ContractId;
            }

            if (contractId == null)
            {
                return NotFound();
            }

            var response = await _contractApiService.DownloadAgreementAsync(contractId.Value);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            var downloadFileName =
                response.Content.Headers.ContentDisposition?.FileNameStar?.Trim('"')
                ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? fileName
                ?? $"Contract-{contractId}-Agreement.pdf";

            return File(fileBytes, "application/pdf", downloadFileName);
        }

        private async Task PopulateClientsAsync(int? selectedClientId = null)
        {
            var clients = await _clientApiService.GetAllAsync();

            ViewData["ClientId"] = new SelectList(
                clients,
                "ClientId",
                "Name",
                selectedClientId);
        }
    }
}