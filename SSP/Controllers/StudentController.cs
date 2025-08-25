using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSP.Data;
using SSP.Models.Domain;
using SSP.Models.ViewModels;
using SSP.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SSP.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly StudyPortalDbContext _dbContext;
        private readonly IYouTubeService _youTubeService;

        public StudentController(StudyPortalDbContext dbContext, IYouTubeService youTubeService)
        {
            _dbContext = dbContext;
            _youTubeService = youTubeService;
        }

        private Guid GetStudentId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == email);
            if (student == null)
                throw new Exception("Student not found");
            return student.S_Id;
        }

        public IActionResult StudentDashboard()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == email);

            if (student == null)
                return RedirectToAction("Login", "Account");

            var homeworks = _dbContext.Homeworks.Where(h => h.S_Id == student.S_Id).ToList();
            var todos = _dbContext.Todos.Where(t => t.S_Id == student.S_Id).ToList();
            var mostWatchedVideoId = _youTubeService.GetMostWatchedVideo(student.S_Id);

            ViewBag.StudentName = student.S_Name;
            ViewBag.StudentId = student.S_Id;
            ViewBag.Homeworks = homeworks;
            ViewBag.Todos = todos;
            ViewBag.MostWatchedVideo = mostWatchedVideoId;

            return View("StudentDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> SaveHomework(Homework homework)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // Get the current logged-in user's email
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.S_Email == userEmail);

            if (student == null)
            {
                return Unauthorized();
            }

            if (homework.HomeworkId == 0) // New homework entry
            {
                homework.S_Id = student.S_Id; // Assign the student's ID to the homework entry

                // ✅ Set DueDate to today if not provided
                if (homework.DueDate == null)
                {
                    homework.DueDate = DateTime.Today;
                }

                _dbContext.Homeworks.Add(homework);
                TempData["SuccessMessage"] = "Homework successfully added!";
            }
            else // Update existing homework entry
            {
                var existing = await _dbContext.Homeworks.FindAsync(homework.HomeworkId);
                if (existing != null && existing.S_Id == student.S_Id)
                {
                    existing.Subject = homework.Subject;
                    existing.Title = homework.Title;
                    existing.Description = homework.Description;

                    // ✅ If DueDate is null, keep existing OR set to today
                    if (homework.DueDate == null)
                    {
                        existing.DueDate = existing.DueDate ?? DateTime.Today;
                    }
                    else
                    {
                        existing.DueDate = homework.DueDate;
                    }

                    existing.IsFinished = homework.IsFinished;
                    TempData["SuccessMessage"] = "Homework successfully updated!";
                }
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Homework"); // Redirect back to the Homework view after saving
        }


        [HttpPost]
        public async Task<IActionResult> MarkHomeworkFinished(int homeworkId)
        {
            var homework = await _dbContext.Homeworks.FindAsync(homeworkId);
            if (homework != null)
            {
                homework.IsFinished = true;
                await _dbContext.SaveChangesAsync();
            }

            return Json(new { success = true, homeworkId });
        }

        public IActionResult Homework()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard", "Admin");

            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == email);
            if (student == null)
                return RedirectToAction("Login", "Account");

            var homeworks = _dbContext.Homeworks.Where(h => h.S_Id == student.S_Id).ToList();
            return View(homeworks);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHomework(int homeworkId)
        {
            var homework = await _dbContext.Homeworks.FindAsync(homeworkId);
            if (homework == null)
                return Json(new { success = false, message = "Homework not found." });

            _dbContext.Homeworks.Remove(homework);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        public IActionResult Record()
        {
            var studentId = GetStudentId(); // Get current student's ID

            var homeworks = _dbContext.Homeworks.Where(h => h.S_Id == studentId).ToList();
            var todos = _dbContext.Todos.Where(t => t.S_Id == studentId).ToList();

            var viewModel = new StudentRecordViewModel
            {
                Homeworks = homeworks,
                Todos = todos
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int homeworkId, bool isFinished)
        {
            var homework = await _dbContext.Homeworks.FindAsync(homeworkId);
            if (homework == null)
                return Json(new { success = false, message = "Homework not found." });

            homework.IsFinished = isFinished;
            await _dbContext.SaveChangesAsync();


            return Json(new { success = true });
        }

        public IActionResult ToDo()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard", "Admin");

            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == email);
            if (student == null)
                return RedirectToAction("Login", "Account");

            var todos = _dbContext.Todos.Where(t => t.S_Id == student.S_Id).ToList();
            return View(todos);
        }

        public IActionResult YouTube()
        {
            var studentId = GetStudentId();
            var mostWatchedVideo = _youTubeService.GetMostWatchedVideo(studentId);
            if (string.IsNullOrEmpty(mostWatchedVideo))
                mostWatchedVideo = "dQw4w9WgXcQ";

            ViewBag.MostWatchedVideo = mostWatchedVideo;
            return View();
        }

        [HttpPost]
        public IActionResult SetMostWatchedVideo(string videoId)
        {
            var studentId = GetStudentId();
            _youTubeService.SetMostWatchedVideo(studentId, videoId);
            return Ok();
        }

        public IActionResult Wikipedia()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard", "Admin");

            return View();
        }

        public IActionResult Books()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard", "Admin");

            return View();
        }
    }
}
