using System;
using System.ComponentModel.DataAnnotations;

namespace SSP.Models.Domain
{
    public class Admin
    {
        [Key]  // Primary Key
        public Guid A_Id { get; set; } = Guid.NewGuid();

        [Required]
        public string A_Name { get; set; }  // ✅ Ensure this exists

        [Required]
        public string A_Email { get; set; }

        [Required]
        public string A_Password { get; set; }

    }
}
