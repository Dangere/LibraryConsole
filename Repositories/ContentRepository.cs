
using System.Data.Common;
using Dapper;
using LibraryManagementSystem.Interfaces;
using MySql.Data.MySqlClient;
namespace LibraryManagementSystem.Repositories;
public class ContentRepository(string connectionString) : IContentRepository
{
    private readonly string _connectionString = connectionString;

    public async Task<bool> AddBookContent(int bookId, string content)
    {
        using DbConnection dbConnection = new MySqlConnection(_connectionString);

        string insertQuery = "INSERT INTO books_content VALUES (@bookId, @content)";

        int rowsAffected = await dbConnection.ExecuteAsync(insertQuery, new { bookId, content });

        return rowsAffected > 0;
    }

    public async Task<bool> BookHasContent(int id)
    {
        using DbConnection dbConnection = new MySqlConnection(_connectionString);

        string scalarQuery = "SELECT COUNT(book_id) FROM books_content WHERE book_id = @id";

        int contentAvailable = await dbConnection.ExecuteScalarAsync<int>(scalarQuery, new { id });

        return contentAvailable > 0;
    }

    public async Task<string?> GetBookContent(int id)
    {
        using DbConnection dbConnection = new MySqlConnection(_connectionString);

        string selectQuery = "SELECT content FROM books_content WHERE book_id = @id";

        return await dbConnection.QueryFirstOrDefaultAsync<string>(selectQuery, new { id });

    }

    public async Task<bool> UpdateBookContent(int bookId, string content)
    {
        using DbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = "UPDATE books_content SET content = @content WHERE book_id = @bookId";

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, new { bookId, content });

        return rowsAffected > 0;
    }
}