using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSP.Models.Domain
{
    public class Student
    {
        [Key]
        public Guid S_Id { get; set; }
        public string S_Name { get; set; }
        public string S_Email { get; set; }
        public string S_Password { get; set; }

        // Reset password fields
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // Navigation properties
        public ICollection<Homework> Homeworks { get; set; }
        public ICollection<Todo> Todos { get; set; }
    }
}
