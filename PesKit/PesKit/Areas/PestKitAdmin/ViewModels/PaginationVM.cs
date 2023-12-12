namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class PaginationVM <T>
    {
        public int CurrentPage { get; set; }
        public double TotalPage { get; set; }
        public List<T> Items { get; set; }
    }
}
