
using System.Data;
using Dapper;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem.Repositories;
public class BookRepository(string connectionString) : IBookRepository
{
    private readonly string _connectionString = connectionString;

    public int AddBook(string title, string author, string genre)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string insertQuery = @"INSERT INTO books (title, author, genre) VALUES (@title, @author, @genre);
        SELECT LAST_INSERT_ID();";

        return dbConnection.ExecuteScalar<int>(insertQuery, new { title, author, genre });

    }

    public bool DeleteBook(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string deleteQuery = "DELETE FROM books WHERE id = @id";

        return dbConnection.Execute(deleteQuery, new { id }) > 0;
    }

    public List<BookEntity> GetAllBooks(int itemsPerPage, int page)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT * FROM books
                            ORDER BY creation_date DESC 
                            LIMIT @itemsPerPage OFFSET @offset";

        int offset = (page - 1) * itemsPerPage;

        return dbConnection.Query<BookEntity>(selectQuery, new { itemsPerPage, offset }).ToList();
    }

    public BookEntity? GetBookById(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = "SELECT * FROM books WHERE id = @id";


        BookEntity? fetchedBook = dbConnection.QuerySingleOrDefault<BookEntity>(selectQuery, new { id });


        return fetchedBook;
    }

    public string GetBookContent(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = "SELECT content FROM books WHERE id = @id";


        string fetchedContent = dbConnection.QueryFirst<string>(selectQuery, new { id });


        return fetchedContent;
    }

    public bool UpdateBook(int bookId, string title, string author, string genre)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string updateQuery = "UPDATE books SET title = @title, author = @author, genre = @genre WHERE id = @bookId";

        return dbConnection.Execute(updateQuery, new { bookId, title, author, genre }) > 0;
    }
}