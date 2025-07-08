using Apibookstore.Data;
using Apibookstore.models;
using Microsoft.EntityFrameworkCore;

namespace Apibookstore.Service
{
    public class AuthService
    {
        private readonly AppDataDbContext _context;
        public AuthService(AppDataDbContext context)
        {
            _context = context;
        }
        public async Task<Booksusers?> RegisterUserAsync(string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == username))
            {
                return null; // User already exists
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new Booksusers
            {
                UserName = username,
                passwordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        public async Task<Booksusers?> LoginUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return null; // User not found
            }
            if (!VerifyPasswordHash(password, user.passwordHash, user.PasswordSalt))
            {
                return null; // Password is correct
            }return user;
        }
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);

        }

        //    using var hmac = new System.Security.Cryptography.HMACSHA512();
        //    var user = new models.Booksusers
        //    {
        //        UserName = username,
        //        PasswordSalt = hmac.Key,
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
        //    };
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}
    }
}
