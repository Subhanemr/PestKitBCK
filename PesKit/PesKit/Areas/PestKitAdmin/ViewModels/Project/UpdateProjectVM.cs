using PesKit.Models;
using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class UpdateProjectVM
    {
        [Required(ErrorMessage = "Title must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Name { get; set; }
        public List<ProjectImage>? ProjectImages { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? HoverPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<int>? ImageIds { get; set; }
    }
}
