using System.ComponentModel.DataAnnotations;

namespace Apibookstore.models
{
    public class BooksDetails
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: If you want to track which user created the book
        // public int? UserId { get; set; }
        // public Booksusers? User { get; set; }
    }
}