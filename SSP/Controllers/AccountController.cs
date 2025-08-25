using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SSP.Data;
using SSP.Models.Domain;
using SSP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SSP.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudyPortalDbContext _context;
        private readonly PasswordHasher<Student> _studentHasher;
        private readonly PasswordHasher<Admin> _adminHasher;
        private readonly IEmailSender _emailSender;

        public AccountController(StudyPortalDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            _studentHasher = new PasswordHasher<Student>();
            _adminHasher = new PasswordHasher<Admin>();
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Students.Any(s => s.S_Email == model.S_Email))
            {
                ModelState.AddModelError("S_Email", "This email is already registered.");
                return View(model);
            }

            var student = new Student
            {
                S_Id = Guid.NewGuid(),
                S_Name = model.S_Name,
                S_Email = model.S_Email,
                S_Password = _studentHasher.HashPassword(null, model.S_Password)
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("AdminDashboard", "Admin");

                if (User.IsInRole("Student"))
                    return RedirectToAction("StudentDashboard", "Student");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var admin = _context.Admins.FirstOrDefault(a => a.A_Email == model.Email);
            if (admin != null &&
                _adminHasher.VerifyHashedPassword(admin, admin.A_Password, model.Password) == PasswordVerificationResult.Success)
            {
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.A_Name),
                    new Claim(ClaimTypes.Email, admin.A_Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                var log = new AuditLog
                {
                    UserEmail = admin.A_Email,
                    Action = "Admin Logged In",
                    Timestamp = DateTime.Now
                };
                _context.AuditLogs.Add(log);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("AuditLogId", log.Id);
                return RedirectToAction("AdminDashboard", "Admin");
            }

            var student = _context.Students.FirstOrDefault(s => s.S_Email == model.Email);
            if (student == null)
            {
                ModelState.AddModelError("Email", "Email not registered.");
                return View(model);
            }

            if (_studentHasher.VerifyHashedPassword(student, student.S_Password, model.Password) != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("Password", "Invalid password.");
                return View(model);
            }

            var studentClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, student.S_Name),
                new Claim(ClaimTypes.Email, student.S_Email),
                new Claim(ClaimTypes.Role, "Student")
            };

            var studentIdentity = new ClaimsIdentity(studentClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(studentIdentity));

            HttpContext.Session.SetString("StudentName", student.S_Name);

            var studentLog = new AuditLog
            {
                UserEmail = student.S_Email,
                Action = "Student Logged In",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(studentLog);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("AuditLogId", studentLog.Id);
            return RedirectToAction("StudentDashboard", "Student");
        }

        public async Task<IActionResult> Logout()
        {
            var auditLogId = HttpContext.Session.GetInt32("AuditLogId");
            if (auditLogId.HasValue)
            {
                var log = await _context.AuditLogs.FindAsync(auditLogId.Value);
                if (log != null)
                {
                    log.LogoutTime = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find the student by email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.S_Email == model.Email);
            if (student != null)
            {
                // Generate a unique token
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                student.ResetToken = token;  // Save the token in the database
                student.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);  // Set expiry time for the token (1 hour)
                await _context.SaveChangesAsync();

                // Generate the reset URL
                var resetUrl = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, token = token },
                    protocol: Request.Scheme);  // The protocol will be 'http' or 'https'

                // The message that will be sent in the email
                var message = $"<h1>Click the link below to reset your password:</h1><a href='{resetUrl}'>Reset Password</a>";

                // Send the email
                await _emailSender.SendEmailAsync(student.S_Email, "Reset Password", message);
            }

            // Show a success message (even if the email doesn't exist, we don't want to give that away)
            TempData["ResetSuccess"] = "If your email is registered, a reset link has been sent.";
            return RedirectToAction("Login");
        }


        // Reset Password (Step 2: Reset the Password)
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["ResetError"] = "Invalid reset link.";
                return RedirectToAction("ForgotPassword");
            }

            // Return the ResetPassword view with the email and token
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find the student by email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.S_Email == model.Email);
            if (student == null || student.ResetToken != model.Token || student.ResetTokenExpiry < DateTime.UtcNow)
            {
                TempData["ResetError"] = "Invalid or expired token.";
                return View(model);
            }

            // Hash and save the new password
            student.S_Password = _studentHasher.HashPassword(student, model.NewPassword);
            student.ResetToken = null;  // Clear the token after reset
            student.ResetTokenExpiry = null;  // Clear the expiry
            await _context.SaveChangesAsync();

            TempData["ResetSuccess"] = "Password successfully reset!";
            return RedirectToAction("Login");
        }


        // Send Reset Link (Optional - You can expose this method if you want to send reset link manually)
        [HttpPost]
        public async Task<IActionResult> SendResetLink(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email cannot be empty.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.S_Email == email);
            if (student == null)
                return NotFound("This email is not registered.");

            // Generate a new token only if expired or missing
            if (string.IsNullOrEmpty(student.ResetToken) || student.ResetTokenExpiry < DateTime.UtcNow)
            {
                student.ResetToken = Guid.NewGuid().ToString();
                student.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // 1 hour expiry
                await _context.SaveChangesAsync();
            }

            // 🔗 Reset link using your local IP
            string resetUrl = $"http://10.112.75.204:5057/Account/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(student.ResetToken)}";

            string message = $@"
                <h2>Password Reset Request</h2>
                <p>We received a request to reset your password. If this was you, click the button below:</p>
                <p><a href='{resetUrl}' style='padding: 10px 15px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>This link will expire in 1 hour. If you did not request this, you can safely ignore this email.</p>
                ";

            try
            {
                await _emailSender.SendEmailAsync(email, "Reset Your Student Portal Password", message);
                return Ok("A reset link has been sent to your email address.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email. Error: {ex.Message}");
            }
        }


    }
}