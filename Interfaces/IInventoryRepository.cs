using LibraryManagementSystem.Models.DTOs;
namespace LibraryManagementSystem.Interfaces;

public interface IInventoryRepository
{
    public List<InventoryBookDTO> GetBooksFromInventory(int itemsPerPage, int page, bool excludeUnavailableBooks);
    public InventoryBookDTO? GetBookById(int id, bool excludeUnavailableBooks);

    public bool AddBookToInventory(int id, int quantity, int borrowingPeriod);

    public bool UpdateBook(int bookId, int quantity, int borrowingPeriod);

    public int CopiesOfBookInInventory(int id);

    public bool BookExistsInInventory(int id);

}