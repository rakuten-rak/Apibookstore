using Apibookstore.Data;
using Apibookstore.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Apibookstore.Controllers
{
    public class CreateBookDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty; 
    }

    public class UpdateBookDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }

    [ApiController]
    [Route("api/books")] // Changed from "api/[controller]" to "api/books"
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly AppDataDbContext _context;
        private readonly ILogger<BookController> _logger;

        public BookController(AppDataDbContext context, ILogger<BookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksDetails>>> Get()
        {
            try
            {
                var books = await _context.Books.ToListAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                return StatusCode(500, "An error occurred while retrieving books");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BooksDetails>> Get(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID {BookId}", id);
                return StatusCode(500, "An error occurred while retrieving the book");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BooksDetails>> Post([FromBody] CreateBookDto bookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = new BooksDetails
                {
                    Name = bookDto.Name,
                    Category = bookDto.Category,
                    Price = bookDto.Price,
                    Description = bookDto.Description
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book: {@BookDto}", bookDto);
                return StatusCode(500, $"An error occurred while creating the book: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateBookDto bookDto)
        {
            try
            {
                if (id != bookDto.Id)
                {
                    return BadRequest("Book ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                existingBook.Name = bookDto.Name;
                existingBook.Description = bookDto.Description;
                existingBook.Price = bookDto.Price;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating book with ID {BookId}", id);
                if (!BookExists(id))
                {
                    return NotFound();
                }
                return Conflict("The book was modified by another user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book with ID {BookId}: {@BookDto}", id, bookDto);
                return StatusCode(500, $"An error occurred while updating the book: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {BookId}", id);
                return StatusCode(500, $"An error occurred while deleting the book: {ex.Message}");
            }
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}