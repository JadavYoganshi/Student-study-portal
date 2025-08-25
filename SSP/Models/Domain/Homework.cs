using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSP.Models.Domain
{
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime HomeworkDate { get; set; } = DateTime.Now;

        // Make DueDate nullable so that it can hold null if not provided
        public DateTime? DueDate { get; set; } // Nullable DateTime

        public bool IsFinished { get; set; }

        // Foreign key
        public Guid S_Id { get; set; }

        [ForeignKey("S_Id")]
        public Student Student { get; set; }
    }
}
