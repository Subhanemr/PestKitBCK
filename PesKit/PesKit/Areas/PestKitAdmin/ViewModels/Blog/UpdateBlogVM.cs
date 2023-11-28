using PesKit.Models;
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
        [Range(1, int.MaxValue, ErrorMessage = "Author must be greater than 0 ")]
        public int? AuthorId { get; set; }
        [Required]
        public int CommentCount { get; set; }
        public List<Author>? Authors { get; set; }
        public List<int> TagIds { get; set; }
        public List<Tag>? Tagss { get; set; }
    }
}
