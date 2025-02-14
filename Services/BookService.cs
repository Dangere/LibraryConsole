using System.Text.RegularExpressions;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;

public class BookService(IBookRepository bookRepository)
{

    private readonly IBookRepository _bookRepository = bookRepository;

    public enum BookGenres
    {
        Mystery,
        Horror,
        Adventure,
        Thriller,
        Drama,
        Dystopian,
        Fantasy,
        HistoricalFiction,
        Romance,
        ScienceFiction
    }

    public string[] GetBookGenres()
    {
        string[] genres = Enum.GetNames<BookGenres>();

        //splitting PascalCase into words
        for (int i = 0; i < genres.Length; i++)
        {
            genres[i] = Regex.Replace(genres[i], "(?!^)([A-Z])", " $1");

        }

        return genres;
    }



    public Result<BookEntity> GetBookById(int id)
    {
        BookEntity? book = _bookRepository.GetBookById(id);
        if (book == null)
            return Result<BookEntity>.Error("Book not found");

        return Result<BookEntity>.Success(book);
    }

    public void DeleteBook(int id)
    {
        _bookRepository.DeleteBook(id);
    }

    public List<BookEntity> GetAllBooks(int itemsPerPage, int page)
    {
        return _bookRepository.GetAllBooks(itemsPerPage, page);

    }
}