using System.ComponentModel.DataAnnotations;

namespace Apibookstore.models
{
    public class BooksDetails
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

       
    }
}