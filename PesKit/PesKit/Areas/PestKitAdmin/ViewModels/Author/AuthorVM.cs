using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class AuthorVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}
