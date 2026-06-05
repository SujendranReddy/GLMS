using GLMS.Enums;
using GLMS.Models;
using GLMS.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;
        private readonly IContractApiService _contractApiService;

        public ServiceRequestsController(
            IServiceRequestApiService serviceRequestApiService,
            IContractApiService contractApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
            _contractApiService = contractApiService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestApiService.GetAllAsync();

            return View(serviceRequests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateContractsAsync();

            return View(new ServiceRequest());
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var createdRequest = await _serviceRequestApiService.CreateAsync(serviceRequest);

            if (createdRequest == null)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create the service request through the API. Check that the selected contract is not expired or on hold.");

                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            await PopulateContractsAsync(serviceRequest.ContractId);

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var updated = await _serviceRequestApiService.UpdateAsync(id, serviceRequest);

            if (!updated)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update the service request through the API. Check that the selected contract is not expired or on hold.");

                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _serviceRequestApiService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractsAsync(int? selectedContractId = null)
        {
            var contracts = await _contractApiService.GetAllAsync(null, null, null);

            var allowedContracts = contracts
                .Where(c => c.Status != ContractStatus.Expired &&
                            c.Status != ContractStatus.OnHold)
                .ToList();

            ViewData["ContractId"] = new SelectList(
                allowedContracts,
                "ContractId",
                "Description",
                selectedContractId);
        }
    }
}