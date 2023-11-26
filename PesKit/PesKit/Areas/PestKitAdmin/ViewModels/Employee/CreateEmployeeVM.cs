using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class CreateEmployeeVM
    {
        [Required(ErrorMessage = "Name must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Department must be entered mutled")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Position must be entered mutled")]
        public int PositionId { get; set; }
        [Required(ErrorMessage = "Image must be uploaded")]
        public IFormFile? Photo { get; set; }
        public string? InstLink { get; set; }
        public string? TwitLink { get; set; }
        public string? FaceLink { get; set; }
        public string? LinkedLink { get; set; }
    }
}
