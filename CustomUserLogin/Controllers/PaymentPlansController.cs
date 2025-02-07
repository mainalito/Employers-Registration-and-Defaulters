using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomUserLogin.Data;
using CustomUserLogin.Models;

namespace CustomUserLogin.Controllers
{
    public class PaymentPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentPlans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PaymentPlans.Include(p => p.employerDefaulter);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PaymentPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPlan = await _context.PaymentPlans
                .Include(p => p.employerDefaulter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentPlan == null)
            {
                return NotFound();
            }

            return View(paymentPlan);
        }

        // GET: PaymentPlans/Create
        public IActionResult Create()
        {
            ViewData["EmployerDefaulterId"] = new SelectList(_context.EmployerDefaulters, "Id", "Id");
            return View();
        }

        // POST: PaymentPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Installments,Amount,Reasons,Status,EmployerDefaulterId")] PaymentPlan paymentPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployerDefaulterId"] = new SelectList(_context.EmployerDefaulters, "Id", "Id", paymentPlan.EmployerDefaulterId);
            return View(paymentPlan);
        }

        // GET: PaymentPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPlan = await _context.PaymentPlans.FindAsync(id);
            if (paymentPlan == null)
            {
                return NotFound();
            }
            ViewData["EmployerDefaulterId"] = new SelectList(_context.EmployerDefaulters, "Id", "Id", paymentPlan.EmployerDefaulterId);
            return View(paymentPlan);
        }

        // POST: PaymentPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Installments,Amount,Reasons,Status,EmployerDefaulterId")] PaymentPlan paymentPlan)
        {
            if (id != paymentPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentPlanExists(paymentPlan.Id))
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
            ViewData["EmployerDefaulterId"] = new SelectList(_context.EmployerDefaulters, "Id", "Id", paymentPlan.EmployerDefaulterId);
            return View(paymentPlan);
        }

        // GET: PaymentPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPlan = await _context.PaymentPlans
                .Include(p => p.employerDefaulter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentPlan == null)
            {
                return NotFound();
            }

            return View(paymentPlan);
        }

        // POST: PaymentPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentPlan = await _context.PaymentPlans.FindAsync(id);
            if (paymentPlan != null)
            {
                _context.PaymentPlans.Remove(paymentPlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentPlanExists(int id)
        {
            return _context.PaymentPlans.Any(e => e.Id == id);
        }
    }
}
