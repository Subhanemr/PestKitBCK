using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class CreateSlideVM
    {
        [Required(ErrorMessage = "Descriptoin must be entered mutled")]
        [MaxLength(50, ErrorMessage = "It should not exceed 50 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Descriptoin must be entered mutled")]
        [MaxLength(100, ErrorMessage = "It should not exceed 100 characters")]
        public string Subtitle { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
