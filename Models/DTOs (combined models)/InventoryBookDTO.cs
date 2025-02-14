namespace LibraryManagementSystem.Models.DTOs;
public class InventoryBookDTO
{
    public required int BookId { get; set; }
    public required string BookTitle { get; set; }
    public required string BookAuthor { get; set; }
    public required string BookGenre { get; set; }
    public required int CopiesAvailable { get; set; }
    public required int BorrowingPeriod { get; set; }

}