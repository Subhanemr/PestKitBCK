using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class UpdateDepartmentVM
    {
        [Required(ErrorMessage = "Title must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
