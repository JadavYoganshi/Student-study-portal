using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSP.Data;
using SSP.Models.Domain;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SSP.Controllers
{
    [Authorize(Roles = "Student")]
    public class ToDoController : Controller
    {
        private readonly StudyPortalDbContext _dbContext;

        public ToDoController(StudyPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Helper method to get the current student's ID based on their email.
        private Guid GetStudentId()
        {
            var studentEmail = User.FindFirstValue(ClaimTypes.Email);
            var student = _dbContext.Students.FirstOrDefault(s => s.S_Email == studentEmail);
            if (student == null)
            {
                throw new Exception("Student not found.");
            }
            return student.S_Id;
        }

        // Action to get the list of To-Do tasks for the current student.
        public IActionResult Index()
        {
            try
            {
                Guid studentId = GetStudentId();
                var tasks = _dbContext.Todos
                    .Where(t => t.S_Id == studentId)
                    .ToList();

                return View("~/Views/Student/Todo.cshtml", tasks);
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error loading tasks: " + ex.Message });
            }
        }

        // Action to save a new To-Do task
        [HttpPost]
        public async Task<IActionResult> SaveTodo(string task)
        {
            try
            {
                var studentId = GetStudentId();

                if (string.IsNullOrEmpty(task))
                {
                    return Json(new { success = false, message = "Task cannot be empty." });
                }

                var studentExists = _dbContext.Students.Any(s => s.S_Id == studentId);
                if (!studentExists)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                var todo = new Todo
                {
                    Task = task,
                    IsCompleted = false,
                    S_Id = studentId
                };

                _dbContext.Todos.Add(todo);

                // 🔥 Add corresponding Record entry here
                var record = new Record
                {
                    Type = "To-Do",
                    Title = task,
                    Description = "Added to-do task",
                    Date = DateTime.Now,
                    Status = "Pending", // ← Important!
                    S_Id = studentId
                };
                _dbContext.Records.Add(record);

                await _dbContext.SaveChangesAsync();

                var updatedTasks = await _dbContext.Todos
                    .Where(t => t.S_Id == studentId)
                    .Select(t => new
                    {
                        TodoId = t.TodoId,
                        Task = t.Task,
                        IsCompleted = t.IsCompleted
                    })
                    .ToListAsync();

                return Json(new { success = true, tasks = updatedTasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while saving task: " + ex.Message });
            }
        }

        // Action to toggle the completion status of a task
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id, bool isCompleted)
        {
            try
            {
                var todo = await _dbContext.Todos.FirstOrDefaultAsync(x => x.TodoId == id);
                if (todo == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }

                todo.IsCompleted = isCompleted;
                await _dbContext.SaveChangesAsync();

                var updatedTasks = await _dbContext.Todos
                    .Where(t => t.S_Id == todo.S_Id)
                    .ToListAsync();

                return Json(new { success = true, tasks = updatedTasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while toggling completion: " + ex.Message });
            }
        }

        // Action to delete a To-Do task
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _dbContext.Todos.FindAsync(id);
                if (todo == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }

                _dbContext.Todos.Remove(todo);
                await _dbContext.SaveChangesAsync();

                var updatedTasks = await _dbContext.Todos
                    .Where(t => t.S_Id == todo.S_Id)
                    .ToListAsync();

                return Json(new { success = true, tasks = updatedTasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while deleting task: " + ex.Message });
            }
        }

        // Action to edit a To-Do task
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] string task)
        {
            try
            {
                var todo = await _dbContext.Todos.FirstOrDefaultAsync(x => x.TodoId == id);
                if (todo == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }

                if (string.IsNullOrEmpty(task))
                {
                    return Json(new { success = false, message = "Task cannot be empty." });
                }

                todo.Task = task;
                await _dbContext.SaveChangesAsync();

                var updatedTasks = await _dbContext.Todos
                    .Where(t => t.S_Id == todo.S_Id)
                    .ToListAsync();

                return Json(new { success = true, tasks = updatedTasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while editing task: " + ex.Message });
            }
        }
    }
}
