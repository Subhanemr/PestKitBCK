namespace PesKit.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
    }
}
