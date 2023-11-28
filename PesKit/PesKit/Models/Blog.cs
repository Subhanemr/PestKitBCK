namespace PesKit.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string ImgUrl { get; set; }
        public int AuthorId { get; set; }
        public int CommentCount { get; set; }
        public Author Author { get; set; }
        public List<BlogTag> Tags { get; set; }
    }
}
