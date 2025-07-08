namespace Apibookstore.models
{
    public class BooksDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }  // = null;

        public string Author {get; set;} // = null;
        public decimal Price { get; set; }
    }
}
