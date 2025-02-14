using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Utilities;
using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.Services;
public class InventoryService(IInventoryRepository inventoryRepository)
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public List<InventoryBookDTO> GetBooksFromInventory(int itemsPerPage, int page, bool excludeUnavailableBooks)
    {
        return _inventoryRepository.GetBooksFromInventory(itemsPerPage, page, excludeUnavailableBooks);

    }

    public Result<InventoryBookDTO> GetBookById(int id, bool excludeUnavailableBooks)
    {
        InventoryBookDTO? book = _inventoryRepository.GetBookById(id, excludeUnavailableBooks);
        if (book == null)
            return Result<InventoryBookDTO>.Error("Book not found in inventory");

        return Result<InventoryBookDTO>.Success(book);

    }
}