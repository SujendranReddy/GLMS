using GLMS.Data;
using GLMS.Enums;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ICurrencyService currencyService,
            IServiceRequestService serviceRequestService)
        {
            _context = context;
            _currencyService = currencyService;
            _serviceRequestService = serviceRequestService;
        }
        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ServiceRequests.Include(s => s.Contract);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            // Since clients can have similar names and contract names, i combined the client name and contract id
            var contracts = _context.Contracts
                .Select(c => new
                {
                    c.ContractId,
                    DisplayText = c.Client.Name + " — Contract " + c.ContractId
                })
                .ToList();

            ViewData["ContractId"] = new SelectList(
                contracts,
                "ContractId",
                "DisplayText"
            );

            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("ServiceRequestId,ContractId,Description,CostUSD,CreatedDate")]
    ServiceRequest serviceRequest)
        {
            //Checks that the contract actually existed before creating a service
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
            }
            else if (!_serviceRequestService.CanCreateRequest(contract))
            {
                //Prevents them from being created if they are not active
                ModelState.AddModelError("", "Cannot create a service request for an inactive contract.");
            }

            try
            {
                //Converts the amount from USD to ZAR
                serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);
                ModelState.Remove("CostZAR");
            }
            catch
            {
                ModelState.AddModelError("", "Unable to retrieve the exchange rate right now. Please try again.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var contracts = _context.Contracts
                .Select(c => new
                {
                    c.ContractId,
                    DisplayText = c.Client.Name + " - Contract " + c.ContractId
                })
                .ToList();

            ViewData["ContractId"] = new SelectList(
                contracts,
                "ContractId",
                "DisplayText",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            var contracts = _context.Contracts
                .Select(c => new
                {
                    c.ContractId,
                    DisplayText = c.Client.Name + " - Contract " + c.ContractId
                })
                .ToList();

            ViewData["ContractId"] = new SelectList(
                contracts,
                "ContractId",
                "DisplayText",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("ServiceRequestId,ContractId,Description,CostUSD,CreatedDate")]
    ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
            {
                return NotFound();
            }

            var existingRequest = await _context.ServiceRequests.FindAsync(id);

            if (existingRequest == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
            }
            else if (!_serviceRequestService.CanCreateRequest(contract))
            {
                ModelState.AddModelError("", "Cannot assign this service request to an inactive contract.");
            }

            try
            {
                serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);
                ModelState.Remove("CostZAR");
            }
            catch
            {
                ModelState.AddModelError("", "Unable to retrieve the exchange rate right now. Please try again.");
            }

            if (!ModelState.IsValid)
            {
                var contracts = _context.Contracts
                    .Select(c => new
                    {
                        c.ContractId,
                        DisplayText = c.Client.Name + " - Contract " + c.ContractId
                    })
                    .ToList();

                ViewData["ContractId"] = new SelectList(
                    contracts,
                    "ContractId",
                    "DisplayText",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            existingRequest.ContractId = serviceRequest.ContractId;
            existingRequest.Description = serviceRequest.Description;
            existingRequest.CostUSD = serviceRequest.CostUSD;
            existingRequest.CostZAR = serviceRequest.CostZAR;
            existingRequest.CreatedDate = serviceRequest.CreatedDate;

            try
            {
                _context.Update(existingRequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRequestExists(serviceRequest.ServiceRequestId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);
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
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
        }
    }
}
