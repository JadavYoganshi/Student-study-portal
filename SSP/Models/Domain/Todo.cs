using System;

namespace SSP.Models.Domain
{
    public class Todo
    {
        public int TodoId { get; set; }
        public string Task { get; set; }
        public bool IsCompleted { get; set; }

        // Add StudentId property (S_Id)
        public Guid S_Id { get; set; }  // This should link to the Student model.

        public virtual Student Student { get; set; }  // Navigation property
    }
}
