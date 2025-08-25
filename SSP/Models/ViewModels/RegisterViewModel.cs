using System.ComponentModel.DataAnnotations;

namespace SSP.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Full Name")]
        public string S_Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^(?!.*\.\.)(?!.*\.$)(?!^\.)[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.(?!([a-zA-Z]{2,})\.\1$)[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$",
            ErrorMessage = "Please enter a valid email (e.g., example@gmail.com)")]
        public string S_Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&>._!*?])[A-Za-z\d@#$%&>._!*?]{8,}$",
            ErrorMessage = "Password must be at least 8 characters and include 1 uppercase, 1 lowercase, and 1 special character (@, #, $, %, &, >, ., _, !, *, ?).")]
        public string S_Password { get; set; }
    }
}
