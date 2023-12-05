using System.ComponentModel.DataAnnotations;

namespace PesKit.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "User Name must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-255 characters")]
        [MaxLength(255, ErrorMessage = "It should not exceed 1-255 characters")]
        public string UserNameOrEmail { get; set; }
        [Required(ErrorMessage = "Password must be entered mutled")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
    }
}
