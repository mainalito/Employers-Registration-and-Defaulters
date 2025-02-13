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
using CustomUserLogin.Enums;
using OfficeOpenXml;

namespace CustomUserLogin.Controllers
{
    [Authorize]
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public EmployersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
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
        [HttpGet]
        public IActionResult BulkUpload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file selected. Please upload a valid Excel file.";
                return View();
            }

            try
            {
                // Validate file extension
                var allowedExtensions = new[] { ".xls", ".xlsx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Error"] = "Invalid file format. Only .xls and .xlsx files are allowed.";
                    return View();

                }

                // Generate a safe file name with a random unique name
                string fileName = $"{Guid.NewGuid()}{fileExtension}";

                // Define a secure storage path outside wwwroot
                var uploadPath = Path.Combine(_env.ContentRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileSavePath = Path.Combine(uploadPath, fileName);

                // Save the uploaded file
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Ensure the file exists before processing
                if (!System.IO.File.Exists(fileSavePath))
                {
                    TempData["Error"] = "File upload failed. The file could not be saved.";
                    return View();

                }

                // Process the Excel file
                using (var package = new ExcelPackage(new FileInfo(fileSavePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        System.IO.File.Delete(fileSavePath); // Delete file on error
                        TempData["Error"] = "Uploaded file does not contain a valid worksheet.";
                        return View();

                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        System.IO.File.Delete(fileSavePath); // Delete file on error
                        TempData["Error"] = "Uploaded file is empty or contains no data.";
                        return View();

                    }

                    List<Employers> employersList = new List<Employers>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string name = worksheet.Cells[row, 1].Text.Trim();

                        string enrollmentNumber = worksheet.Cells[row, 2].Text.Trim();
                        string ssnitEmployerNumber = worksheet.Cells[row, 3].Text.Trim();
                        string digitalAddress = worksheet.Cells[row, 4].Text.Trim();
                        string email = worksheet.Cells[row, 5].Text.Trim();
                        string phoneNumber = worksheet.Cells[row, 6].Text.Trim();

                        if (string.IsNullOrWhiteSpace(enrollmentNumber) || string.IsNullOrWhiteSpace(name) ||
                            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            TempData["Error"] = $"Missing required fields at row {row}. Skipping entry.";
                            continue;
                        }

                        bool exists = await _context.Employers.AnyAsync(e => e.EnrollmentNumber == enrollmentNumber);
                        if (exists)
                        {
                            TempData["Error"] = $"Duplicate Enrollment Number at row {row}: {enrollmentNumber}. Skipping entry.";
                            continue;
                        }

                        var employer = new Employers
                        {
                            EnrollmentNumber = enrollmentNumber,
                            SSNITEmployerNumber = ssnitEmployerNumber,
                            Name = name,
                            DigitalAddress = digitalAddress,
                            Email = email,
                            PhoneNumber = phoneNumber,
                            Status = EmployerStatus.Submitted,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = _userManager.GetUserId(User),
                            FileName = fileName
                        };

                        employersList.Add(employer);
                    }

                    if (employersList.Count > 0)
                    {
                        await _context.Employers.AddRangeAsync(employersList);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = $"{employersList.Count} records uploaded successfully!";
                    }
                    else
                    {
                        System.IO.File.Delete(fileSavePath); // Delete file if no valid data
                        TempData["Error"] = "No  records found in the file.";
                    }
                }

                return View();
            }
            catch (Exception ex)
            {

                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return View();
            }
        }
        public IActionResult DownloadTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "EmployerRegistration.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Template not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BulkUploadTemplate.xlsx");
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
