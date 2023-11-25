using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class UpdateBlogVM
    {
        [Required(ErrorMessage = "Title must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Descriptoin must be entered mutled")]
        [MaxLength(100, ErrorMessage = "It should not exceed 100 characters")]
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public IFormFile? Photo { get; set; }
        public string ImgUrl { get; set; }
        [Required]
        public int? AuthorId { get; set; }
        [Required]
        public int CommentCount { get; set; }
    }
}
