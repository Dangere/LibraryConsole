using System.Data;
using Dapper;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using MySql.Data.MySqlClient;
namespace LibraryManagementSystem.Repositories;

public class InventoryRepository(string connectionString) : IInventoryRepository
{
    private readonly string _connectionString = connectionString;

    public bool AddBookToInventory(int id, int copiesAvailable, int borrowingPeriod)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string insertQuery = "INSERT INTO inventory (book_id, copies_available, borrowing_period) VALUES (@id, @copiesAvailable, @borrowingPeriod)";
        return dbConnection.Execute(insertQuery, new { id, copiesAvailable, borrowingPeriod }) > 0;
    }

    public bool BookExistsInInventory(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = "SELECT COUNT(book_id) FROM inventory WHERE book_id = @id";

        int numberOfBooks = dbConnection.ExecuteScalar<int>(selectQuery, new { id });
        return numberOfBooks > 0;
    }


    public int CopiesOfBookInInventory(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = "Select copies_available FROM inventory WHERE book_id = @id";

        int availableBookCopies = dbConnection.ExecuteScalar<int?>(selectQuery, new { id }) ?? 0;

        return availableBookCopies;
    }

    public List<InventoryBookDTO> GetBooksFromInventory(int itemsPerPage, int page, bool excludeUnavailableBooks)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT book_id as bookId, title AS bookTitle, author as bookAuthor, genre AS bookGenre, copies_available AS copiesAvailable, borrowing_period as borrowingPeriod 
        FROM inventory
        INNER JOIN books ON book_id = id AND IF (@excludeUnavailableBooks, copies_available , 1) > 0
        ORDER BY book_id DESC LIMIT @itemsPerPage OFFSET @offset";

        int offset = (page - 1) * itemsPerPage;

        List<InventoryBookDTO> bookInventoryDetails = dbConnection.Query<InventoryBookDTO>(selectQuery, new { itemsPerPage, offset, excludeUnavailableBooks }).ToList();

        return bookInventoryDetails;
    }

    public InventoryBookDTO? GetBookById(int id, bool excludeUnavailableBooks)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT book_id as bookId, title AS bookTitle, author as bookAuthor, genre AS bookGenre, copies_available AS copiesAvailable, borrowing_period as borrowingPeriod 
        FROM inventory
        INNER JOIN books ON book_id = id AND IF (@excludeUnavailableBooks, copies_available , 1) > 0
        WHERE book_id = @id";


        InventoryBookDTO? bookInventoryDetails = dbConnection.QueryFirstOrDefault<InventoryBookDTO>(selectQuery, new { id, excludeUnavailableBooks });

        return bookInventoryDetails;
    }

    public bool UpdateBook(int bookId, int newCopies, int newBorrowPeriod)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = @"UPDATE inventory SET copies_available = @newCopies, borrowing_period = @newBorrowPeriod WHERE book_id = @bookId";

        int rowsAffected = dbConnection.Execute(updateQuery, new { bookId, newCopies, newBorrowPeriod });

        return rowsAffected > 0;
    }
}