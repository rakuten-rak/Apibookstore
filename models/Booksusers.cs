namespace Apibookstore.models
{
    public class Booksusers
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
