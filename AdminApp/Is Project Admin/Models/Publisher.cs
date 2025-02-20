namespace Is_Project_Admin.Models
{
    public class Publisher
    {
        public Guid Id { get; set; }
        public string? PublisherName { get; set; }
        public virtual ICollection<Book>? Books { get; set; }
    }
}
