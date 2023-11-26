namespace PesKit.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public string ImgUrl { get; set; }
        public string? InstLink { get; set; }
        public string? TwitLink { get; set; }
        public string? FaceLink { get; set; }
        public string? LinkedLink { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }

    }
}
