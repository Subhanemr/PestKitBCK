namespace PesKit.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public List<SlideImage> Photo { get; set; }
    }
}
