using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;

public class BorrowService(IBorrowRepository borrowRepository, IInventoryRepository inventoryRepository, AuthService authService, BalanceService balanceService)
{

    private readonly IBorrowRepository _borrowRepository = borrowRepository;
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly AuthService _authService = authService;
    private readonly BalanceService _balanceService = balanceService;

    public readonly decimal PenaltyPerDay = 2.00M;

    public Result<string> BorrowBook(InventoryBookDTO inventoryBook)
    {
        //making sure theres a user logged in 
        if (_authService.CurrentUser == null)
        {
            return Result<string>.Error("You need to log in to borrow books");
        }

        //Making sure we aren't borrowing a book that has no copies in the inventory
        if (_inventoryRepository.CopiesOfBookInInventory(inventoryBook.BookId) <= 0)
        {
            return Result<string>.Error("Book isn't available in inventory");

        }

        //Making sure we aren't borrowing a book that is already borrowed by the user
        if (_borrowRepository.IsBookAlreadyBorrowed(inventoryBook.BookId, _authService.CurrentUser.Id))
        {
            return Result<string>.Error("Book is already borrowed");

        }

        try
        {
            _borrowRepository.BorrowBook(inventoryBook.BookId, _authService.CurrentUser.Id, inventoryBook.BorrowingPeriod);
            return Result<string>.Success("Success");
        }
        catch (Exception ex)
        {
            return Result<string>.Error(ex.Message);


        }
    }

    public Result<decimal> ReturnBook(UserBorrowedBookDTO borrowedBookDTO)
    {
        //making sure theres a user logged in 
        if (_authService.CurrentUser == null)
        {
            return Result<decimal>.Error("User must be logged in to return books");
        }

        //Making sure we aren't returning a book that doesn't have data inside the inventory 
        if (!_inventoryRepository.BookExistsInInventory(borrowedBookDTO.BookId))
        {
            return Result<decimal>.Error("Book wasn't ever available in inventory.. how are you returning it");

        }

        //if the book is overdue we decrease the balance to pay the penalty
        if (borrowedBookDTO.OverdueDays > 0)
        {
            Result<decimal> result = _balanceService.AddToCurrentUserBalance(-borrowedBookDTO.Penalty);
            //if there is an error we returns a result object with error to the caller
            if (!result.IsSuccess)
            {
                return Result<decimal>.Error(result.ErrorMessage!);

            }

        }


        try
        {
            _borrowRepository.ReturnBook(borrowedBookDTO.BookId, _authService.CurrentUser.Id);
            return Result<decimal>.Success(_authService.CurrentUser.Balance);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Error(ex.Message);


        }
    }
    public Result<List<UserBorrowedBookDTO>> GetBorrowedBooksForCurrentUser()
    {
        if (_authService.CurrentUser is null)
            return Result<List<UserBorrowedBookDTO>>.Error("User must be logged in to view borrowed books");


        return Result<List<UserBorrowedBookDTO>>.Success(_borrowRepository.GetBorrowedBooks(_authService.CurrentUser.Id));
    }

    public int CopiesBorrowedOfBook(int id)
    {
        return _borrowRepository.CopiesBorrowedOfBook(id);
    }

    public bool IsBookBorrowedByCurrentUser(int bookId)
    {
        if (_authService.CurrentUser == null)
        {
            return false;
        }

        return _borrowRepository.IsBookAlreadyBorrowed(bookId, _authService.CurrentUser.Id);
    }

}