using Microsoft.AspNetCore.Identity;

namespace PesKit.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Img { get; set; } = "default-profile.png";
        public List<BasketItem> BasketItems { get; set; }
    }
}
