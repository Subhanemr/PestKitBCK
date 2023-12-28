using System.ComponentModel.DataAnnotations;

namespace PesKit.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "User Name must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-25 characters")]
        [MaxLength(25, ErrorMessage = "It should not exceed 1-25 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-25 characters")]
        [MaxLength(25, ErrorMessage = "It should not exceed 1-25 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-25 characters")]
        [MaxLength(25, ErrorMessage = "It should not exceed 1-25 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email must be entered mutled")]
        [MinLength(10, ErrorMessage = "It should not exceed 10-255 characters")]
        [MaxLength(255, ErrorMessage = "It should not exceed 25-255 characters")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password must be entered mutled")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password must be entered mutled")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The password must be the same")]
        public string ConfirmPassword { get; set; }
    }
}
