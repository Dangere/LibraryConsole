using LibraryManagementSystem.Models.Entities;
namespace LibraryManagementSystem.Interfaces;

public interface IBookRepository
{
    public int AddBook(string title, string author, string genre);
    public bool UpdateBook(int bookId, string title, string author, string genre);
    public bool DeleteBook(int id);
    public BookEntity? GetBookById(int id);
    public List<BookEntity> GetAllBooks(int itemsPerPage, int page);

    public string GetBookContent(int id);
}