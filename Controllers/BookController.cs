using Apibookstore.Data;
using Apibookstore.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Apibookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly AppDataDbContext _context;
        public BookController(AppDataDbContext context) => _context = context;

        //[HttpGet("api/books")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksDetails>>> Get() => 
            await _context.Books.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<BooksDetails>> Get(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }
        [HttpPost]
        public async Task<ActionResult<BooksDetails>> Post(BooksDetails book)
        {
            if (book == null)
            {
                return BadRequest("Book details cannot be null");
            }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, BooksDetails book)
        {
            if (id != book.Id)
            {
                return BadRequest("Book ID mismatch");
            }
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
             
                
                throw;
                
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        }
}
