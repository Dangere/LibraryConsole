namespace LibraryManagementSystem.Models.Entities;
public class InventoryBookEntity
{
    public required int BookId { get; set; }
    public required int Quantity { get; set; }
    public required int BorrowingPeriod { get; set; }
}