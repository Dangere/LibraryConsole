namespace LibraryManagementSystem.Models.DTOs;
public class UserBorrowedBookDTO
{
    public required int BookId { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Genre { get; set; }
    public required DateTime BorrowingDate { get; set; }
    public required DateTime ReturnDate { get; set; }
    public required int DaysLeft { get; set; }
    public required int OverdueDays { get; set; }
    public required decimal Penalty { get; set; }

}