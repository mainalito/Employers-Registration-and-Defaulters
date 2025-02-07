using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CustomUserLogin.Data;
using CustomUserLogin.Models;
using CustomUserLogin.ViewModel;

namespace CustomUserLogin.Controllers
{
    [Authorize]
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            int TotalDefaulters = await _context.EmployerDefaulters.CountAsync();
            float TotalAmountDue = await _context.EmployerDefaulters.SumAsync(e => (float)e.TotalAmountDue);
            int totalEmployers = await _context.Employers.CountAsync();
            int PaymentPlans = await _context.PaymentPlans.CountAsync();

            var employers = await _context.Employers
                .AsNoTracking()
                .Select(e => new Employers
                {
                    Id = e.Id,
                    Name = e.Name,
                    EnrollmentNumber = e.EnrollmentNumber,
                    SSNITEmployerNumber = e.SSNITEmployerNumber,
                    DigitalAddress = e.DigitalAddress,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Status = e.Status,
                    CreatedDate = e.CreatedDate,
                    CreatedBy = _context.Users
                        .Where(u => u.Id == e.CreatedBy)
                        .Select(u => u.Name) // Fetch Username instead of ID
                        .FirstOrDefault()
                })
                .ToListAsync();
            // Pass count to ViewBag
            ViewBag.TotalDefaulters = TotalDefaulters;
            ViewBag.TotalAmountDue = TotalAmountDue;
            ViewBag.TotalEmployers = totalEmployers;
            ViewBag.PaymentPlans = PaymentPlans;
            return View(employers);
        }



        // ✅ View Employer Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employer = await _context.Employers.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null) return NotFound();
            // Get all defaulters related to this employer, ordered by Id DESC
            ViewBag.EmployerDefaulters = await _context.EmployerDefaulters
                .Where(ed => ed.EmployerId == employer.Id)
                .OrderByDescending(ed => ed.Id)  // Order by Id DESC
                .ToListAsync();

            return employer == null ? NotFound() : View(employer);
        }

        // ✅ Show Create Form
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Handle Employer Creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnrollmentNumber,SSNITEmployerNumber,DigitalAddress,Email,PhoneNumber")] Employers employer)
        {
            if (ModelState.IsValid)
            {
                employer.Status = Enums.EmployerStatus.Submitted;
                employer.CreatedDate = DateTime.UtcNow;
                employer.CreatedBy = _userManager.GetUserId(User);
                Console.WriteLine(employer.CreatedBy);


                _context.Employers.Add(employer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                {
                    // Log validation errors to console or debugging
                    foreach (var modelStateKey in ModelState.Keys)
                    {
                        var modelStateVal = ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            Console.WriteLine($"Validation Error: {modelStateKey} - {error.ErrorMessage}");
                        }
                    }
                }


                return View(employer);
            }
        }

        // ✅ Show Edit Form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employer = await _context.Employers.FindAsync(id);
            if (employer == null) return NotFound();

            return View(employer);
        }

        // ✅ Handle Employer Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnrollmentNumber,SSNITEmployerNumber,DigitalAddress,Email,PhoneNumber")] Employers employer)
        {
            if (id != employer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployer = await _context.Employers.FindAsync(id);
                    if (existingEmployer == null) return NotFound();

                    // Update only allowed fields
                    existingEmployer.Name = employer.Name;
                    existingEmployer.EnrollmentNumber = employer.EnrollmentNumber;
                    existingEmployer.SSNITEmployerNumber = employer.SSNITEmployerNumber;
                    existingEmployer.DigitalAddress = employer.DigitalAddress;
                    existingEmployer.Email = employer.Email;
                    existingEmployer.PhoneNumber = employer.PhoneNumber;
                    existingEmployer.CreatedBy = _userManager.GetUserId(User);
                    Console.WriteLine(existingEmployer.CreatedBy);

                    _context.Update(existingEmployer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployersExists(employer.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(employer);
        }

        // ✅ Show Delete Confirmation Page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employer = await _context.Employers.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return employer == null ? NotFound() : View(employer);
        }

        // ✅ Handle Employer Deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employer = await _context.Employers.FindAsync(id);
            if (employer == null) return NotFound();

            _context.Employers.Remove(employer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Check If Employer Exists
        private bool EmployersExists(int id)
        {
            return _context.Employers.Any(e => e.Id == id);
        }
    }
}
