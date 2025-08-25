using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSP.Data;
using SSP.Models.Domain;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SSP.Controllers
{
    [Authorize(Roles = "Student")]
    public class HomeworkController : Controller
    {
        private readonly StudyPortalDbContext _dbContext;

        public HomeworkController(StudyPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private Guid GetStudentId()
        {
            var email = User.Identity?.Name;
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == email);
            return student?.S_Id ?? Guid.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var studentId = GetStudentId();
            var homeworkList = _dbContext.Homeworks
                .Where(h => h.S_Id == studentId)
                .ToList();

            return View("Homework", homeworkList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int homeworkId, bool isFinished)
        {
            var homework = await _dbContext.Homeworks.FindAsync(homeworkId);

            if (homework != null)
            {
                homework.IsFinished = isFinished;

                _dbContext.Homeworks.Update(homework);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Homework not found." });
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Homework homework)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    homework.HomeworkId = 0;
                    homework.S_Id = GetStudentId();

                    if (homework.S_Id == Guid.Empty)
                    {
                        ModelState.AddModelError("", "Student not found.");
                        return View(homework);
                    }

                    _dbContext.Homeworks.Add(homework);

                    var recordEntry = new Record
                    {
                        Type = "Homework",
                        Title = homework.Title,
                        Description = homework.Description,
                        Date = DateTime.Now
                    };

                    _dbContext.Records.Add(recordEntry);

                    await _dbContext.SaveChangesAsync();

                    var updatedHomeworkList = _dbContext.Homeworks
                        .Where(h => h.S_Id == homework.S_Id)
                        .ToList();

                    return PartialView("_HomeworkListPartial", updatedHomeworkList);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return View(homework);
        }

        [HttpPost]
        public IActionResult AddFromDashboard(Homework model)
        {
            if (ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var student = _dbContext.Students.FirstOrDefault(x => x.S_Email == email);

                if (student != null)
                {
                    model.S_Id = student.S_Id;
                    _dbContext.Homeworks.Add(model);

                    var recordEntry = new Record
                    {
                        Type = "Homework",
                        Title = model.Title,
                        Description = model.Description,
                        Date = DateTime.Now
                    };

                    _dbContext.Records.Add(recordEntry);
                    _dbContext.SaveChanges();

                    return RedirectToAction("Welcome", "Student");
                }
            }

            return RedirectToAction("Welcome", "Student");
        }
        [HttpPost]
        public async Task<IActionResult> SaveHomework(Homework model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var studentId = GetStudentId();
            if (studentId == Guid.Empty)
                return RedirectToAction("Index", "Homework");

            // Ensure DueDate is set to today's date if it is not provided (null).
            if (!model.DueDate.HasValue)
            {
                model.DueDate = DateTime.Today;  // Set DueDate to today's date if it is null
            }

            if (model.HomeworkId == 0)
            {
                // New homework entry
                model.S_Id = studentId;

                // Add homework to database
                _dbContext.Homeworks.Add(model);

                // Add a record of the homework creation
                _dbContext.Records.Add(new Record
                {
                    Type = "Homework",
                    Title = model.Title,
                    Description = model.Description,
                    Date = DateTime.Now
                });

                // Save the changes to the database
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // Update existing homework entry
                var existing = await _dbContext.Homeworks.FindAsync(model.HomeworkId);
                if (existing != null && existing.S_Id == studentId)
                {
                    existing.Subject = model.Subject;
                    existing.Title = model.Title;
                    existing.Description = model.Description;
                    existing.DueDate = model.DueDate;  // Ensure DueDate is updated
                    existing.IsFinished = model.IsFinished;

                    // Update homework entry
                    _dbContext.Homeworks.Update(existing);

                    // Log the edit action in the records
                    _dbContext.Records.Add(new Record
                    {
                        Type = "Homework Edit",
                        Title = existing.Title,
                        Description = "Updated homework.",
                        Date = DateTime.Now
                    });

                    // Save the changes
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Homework added successfully!";
                }
            }

            // Redirect to the Homework index page after save/update
            return RedirectToAction("Index", "Homework");
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(Homework model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Homework");
            }

            var studentId = GetStudentId();
            if (studentId == Guid.Empty)
                return RedirectToAction("Index", "Homework");

            if (model.HomeworkId == 0)
            {
                model.S_Id = studentId;

                _dbContext.Homeworks.Add(model);
                _dbContext.Records.Add(new Record
                {
                    Type = "Homework",
                    Title = model.Title,
                    Description = model.Description,
                    Date = DateTime.Now
                });

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var existing = await _dbContext.Homeworks.FindAsync(model.HomeworkId);
                if (existing != null && existing.S_Id == studentId)
                {
                    existing.Subject = model.Subject;
                    existing.Title = model.Title;
                    existing.Description = model.Description;
                    existing.DueDate = model.DueDate;
                    existing.IsFinished = model.IsFinished;

                    await _dbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Homework");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int homeworkId)
        {
            var studentId = GetStudentId();

            if (studentId == Guid.Empty)
                return Json(new { success = false, message = "Student not found." });

            var homework = await _dbContext.Homeworks
                .FirstOrDefaultAsync(h => h.HomeworkId == homeworkId && h.S_Id == studentId);

            if (homework == null)
            {
                return Json(new { success = false, message = $"Homework not found with ID: {homeworkId}" });
            }

            // Delete homework
            _dbContext.Homeworks.Remove(homework);

            // Also remove related records from Record table
            var record = await _dbContext.Records
                .Where(r => r.Title == homework.Title &&
                            r.Description == homework.Description &&
                            r.Type == "Homework")
                .ToListAsync();

            _dbContext.Records.RemoveRange(record);

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }


    }

}
