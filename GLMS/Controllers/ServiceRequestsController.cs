using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS.Data;
using GLMS.Models;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "Description");
            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("ServiceRequestId,ContractId,Description,CostUSD,CostZAR,CreatedDate")]
    ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
            }
            else if (contract.Status == Enums.ContractStatus.Expired ||
                     contract.Status == Enums.ContractStatus.OnHold)
            {
                ModelState.AddModelError("",
                    "Cannot create a service request for an inactive contract.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] = new SelectList(
                _context.Contracts,
                "ContractId",
                "Description",
                serviceRequest.ContractId);

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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "Description", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceRequestId,ContractId,Description,CostUSD,CostZAR,CreatedDate")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "Description", serviceRequest.ContractId);
            return View(serviceRequest);
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
