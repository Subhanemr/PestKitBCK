using System.ComponentModel.DataAnnotations;

namespace PesKit.ViewModels
{
    public class EditUserVM
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
        public string Img { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
