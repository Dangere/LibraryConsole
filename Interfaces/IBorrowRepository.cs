using LibraryManagementSystem.Models.DTOs;
namespace LibraryManagementSystem.Interfaces;

public interface IBorrowRepository
{

    public decimal PenaltyPerDay();

    public void BorrowBook(int bookId, int userId, int borrowPeriod);

    public List<UserBorrowedBookDTO> GetBorrowedBooks(int userId);


    public bool IsBookAlreadyBorrowed(int bookId, int userId);

    public void ReturnBook(int bookId, int userId);

    public int CopiesBorrowedOfBook(int id);


}