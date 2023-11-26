using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class CreateUpdatePositionVM
    {
        [Required(ErrorMessage = "Title must be entered mutled")]
        [MaxLength(50, ErrorMessage = "It should not exceed 25 characters")]
        public string Name { get; set; }
    }
}
