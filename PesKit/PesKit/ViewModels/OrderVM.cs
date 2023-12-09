using PesKit.Models;
using System.ComponentModel.DataAnnotations;

namespace PesKit.ViewModels
{
    public class OrderVM
    {
        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
    }
}
