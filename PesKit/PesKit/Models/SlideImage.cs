namespace PesKit.Models
{
    public class SlideImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool? IsPrimary { get; set; }
        public Slide Slide { get; set; }
    }
}
