using System.Data;
using Dapper;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using MySql.Data.MySqlClient;
namespace LibraryManagementSystem.Repositories;

public class BorrowRepository(string connectionString) : IBorrowRepository
{
    private readonly string _connectionString = connectionString;

    public void BorrowBook(int bookId, int userId, int borrowDaysPeriod)
    {
        //we do a check if the book exists inside the inventory
        //theres a trigger that decreases the available copies column on the inventory table when a book is inserted into the borrowed_books
        string insertQuery =
        @"INSERT INTO borrowed_books (book_id, user_id, return_date)
            VALUES (
                    @bookId,
                    @userId,
                    DATE_ADD(
                        CURDATE(),
                        INTERVAL @borrowDaysPeriod DAY
                    )
                );";

        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        dbConnection.Execute(insertQuery, new { bookId, userId, borrowDaysPeriod });
    }

    public int CopiesBorrowedOfBook(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string selectQuery = @"SELECT COUNT(book_id)
                                FROM borrowed_books
                                WHERE book_id = @id;";

        int copiesBorrowed = dbConnection.ExecuteScalar<int>(selectQuery, new { id });

        return copiesBorrowed;

    }

    public List<UserBorrowedBookDTO> GetBorrowedBooks(int userId)
    {
        decimal penaltyPerDay = PenaltyPerDay();

        string selectQuery =
        @"SELECT book_id AS BookId, books.author AS Author, books.title AS Title, books.genre AS Genre, borrowing_date AS BorrowingDate, return_date AS ReturnDate, GREATEST( DATEDIFF(CURRENT_DATE(), return_date), 0 ) AS OverdueDays,
        CONVERT( GREATEST ( DATEDIFF(CURRENT_DATE(), return_date) * @penaltyPerDay, 0 ), DECIMAL(5, 2) ) AS Penalty,
        GREATEST( DATEDIFF(return_date, CURRENT_DATE()), 0 ) AS DaysLeft
        FROM borrowed_books
        INNER JOIN books ON book_id = books.id
        WHERE user_id = @userId;";

        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        List<UserBorrowedBookDTO> borrowedBookDTOs = [.. dbConnection.Query<UserBorrowedBookDTO>(selectQuery, new { userId, penaltyPerDay })];

        return borrowedBookDTOs;
    }

    public bool IsBookAlreadyBorrowed(int bookId, int userId)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT COUNT(book_id) FROM borrowed_books WHERE book_id = @bookId and user_id = @userId";

        int numberOfBooks = dbConnection.ExecuteScalar<int>(selectQuery, new { bookId, userId });

        return numberOfBooks > 0;
    }

    public decimal PenaltyPerDay()
    {
        return 2.00M;
    }

    public void ReturnBook(int bookId, int userId)
    {
        //we do a check if the book exists inside the inventory
        //we have a trigger to increase the available copies of the book inside the inventory
        //when a book is returned
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string deleteQuery = @"DELETE FROM borrowed_books WHERE book_id = @bookId AND user_id = @userId limit 1";

        dbConnection.Execute(deleteQuery, new { bookId, userId });
    }

}