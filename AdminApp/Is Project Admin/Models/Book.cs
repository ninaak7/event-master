using System.ComponentModel.DataAnnotations;

namespace Is_Project_Admin.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string? BookName { get; set; }
        public string? BookDescription { get; set; }
        public string? BookImage { get; set; }
        public int? Price { get; set; }
        public int? Rating { get; set; }
        public Guid? AuthorId { get; set; }
        public Author? Author { get; set; }
        public Guid? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
    }
}
