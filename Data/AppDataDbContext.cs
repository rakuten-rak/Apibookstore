using Apibookstore.models;
using Microsoft.EntityFrameworkCore;

namespace Apibookstore.Data
{
    public class AppDataDbContext:DbContext
    {
        public DbSet<Booksusers> Users => Set<Booksusers>();
        public DbSet<BooksDetails> Books => Set<BooksDetails>();
        public AppDataDbContext(DbContextOptions<AppDataDbContext> options):base(options) { }

    }
}
