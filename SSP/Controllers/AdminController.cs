using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSP.Data;
using SSP.Models.Domain;
using SSP.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SSP.Controllers
{
    public class AdminController : Controller
    {
        private readonly StudyPortalDbContext _context;

        public AdminController(StudyPortalDbContext context)
        {
            _context = context;
        }

        // 🏠 Admin Dashboard (Shows Registered Students Only)
        public IActionResult AdminDashboard()
        {
            // ✅ Fetch only students (excluding Admins)
            var students = _context.Students
                .Where(s => s.S_Email != "admin@example.com") // Exclude Admins
                .ToList() ?? new List<Student>(); // ✅ Prevent null issues

            var model = new AdminDashboardViewModel
            {
                Students = students
            };

            return View(model);
        }

        // 📜 View Audit Logs Page
        public IActionResult ViewAuditLogs()
        {
            var logs = _context.AuditLogs
                .OrderByDescending(log => log.Timestamp)
                .ToList() ?? new List<AuditLog>(); // ✅ Prevent null issues

            return View("ViewAuditLogs", logs); // ✅ Ensure the correct view is returned
        }
    }
}
