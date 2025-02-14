
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;
public class AdminService(IUserRepository userRepository, IBalanceRepository balanceRepository, IBorrowRepository borrowRepository, IBookRepository bookRepository, IContentRepository contentRepository, IInventoryRepository inventoryRepository, AuthService authService)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IBalanceRepository _balanceRepository = balanceRepository;
    private readonly IBorrowRepository _borrowRepository = borrowRepository;
    private readonly IBookRepository _bookRepository = bookRepository;
    private readonly IContentRepository _contentRepository = contentRepository;

    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly AuthService _authService = authService;


    public List<UserEntity> GetAllUsers(int itemsPerPage, int page) => _userRepository.GetAllUsers(itemsPerPage, page);

    public Result<UserEntity> GetUserById(int id)
    {
        if (!IsCurrentUserAdmin())
            return Result<UserEntity>.Error("Admin must be logged in to make this operation");

        UserEntity? fetchedUser = _userRepository.GetUserById(id);
        if (fetchedUser == null)
            return Result<UserEntity>.Error("User not found");

        return Result<UserEntity>.Success(fetchedUser);

    }
    public Result<string> UpdateUserEmail(int userId, string currentEmail, string newEmail)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        if (!Validators.ValidateEmail(newEmail))
            return Result<string>.Error("Invalid email format");

        if (currentEmail == newEmail)
            return Result<string>.Error("New email can't be the same as the old one");

        if (_authService.EmailIsUsed(newEmail))
            return Result<string>.Error("Email already in use by another user");

        if (_userRepository.UpdateUserEmail(userId, newEmail))
        {
            return Result<string>.Success("Email updated successfully");
        }
        else
        {
            return Result<string>.Error("Failed to update email");
        }

    }
    public Result<string> UpdateUserPhone(int userId, string? currentPhone, string newPhone)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        if (!Validators.ValidatePhone(newPhone))
            return Result<string>.Error("Invalid phone number format");

        if (currentPhone == newPhone && currentPhone != null)
            return Result<string>.Error("New phone number can't be the same as the old one");


        if (_userRepository.UpdateUserPhone(userId, newPhone))
        {
            return Result<string>.Success("Email updated successfully");
        }
        else
        {
            return Result<string>.Error("Failed to update email");
        }

    }
    public Result<string> UpdateUserPassword(int userId, string salt, string currentHash, string newPassword)
    {

        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        if (!Validators.ValidatePassword(newPassword))
            return Result<string>.Error("Invalid password format");

        string new_hashed_password = Hashing.HashPassword(newPassword, Convert.FromBase64String(salt));

        if (currentHash == new_hashed_password)
            return Result<string>.Error("new password can't be the same as the old one");

        if (_userRepository.UpdateUserPassword(userId, new_hashed_password))
        {

            return Result<string>.Success("Password updated successfully");
        }
        else
        {
            return Result<string>.Error("Failed to update password");
        }
    }

    public Result<string> UpdateUserFirstAndLastName(int userId, string firstName, string lastName)
    {

        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        if (!Validators.ValidateName(firstName))
            return Result<string>.Error("Invalid name format in first name");
        if (!Validators.ValidateName(lastName))
            return Result<string>.Error("Invalid name format in last name");


        if (_userRepository.UpdateUserFirstAndLastName(userId, firstName, lastName))
        {

            return Result<string>.Success("Name updated successfully");
        }
        else
        {
            return Result<string>.Error("Failed to update name");
        }
    }


    public Result<decimal> AddUserBalance(int userId, decimal currentBalance, decimal balanceChange)
    {
        decimal newBalance = currentBalance + balanceChange;

        if (!IsCurrentUserAdmin())
            return Result<decimal>.Error("Admin must be logged in to make this operation");

        if (newBalance < 0)
            return Result<decimal>.Error("Not enough balance to remove");


        _balanceRepository.UpdateBalance(userId, newBalance);
        _balanceRepository.AddTransaction(userId, _authService.CurrentUser!.Id, balanceChange, newBalance);

        return Result<decimal>.Success(balanceChange);
    }
    public Result<List<UserBorrowedBookDTO>> GetBorrowedBooksForUser(int userId)
    {
        if (!IsCurrentUserAdmin())
            return Result<List<UserBorrowedBookDTO>>.Error("Admin must be logged in to make this operation");

        return Result<List<UserBorrowedBookDTO>>.Success(_borrowRepository.GetBorrowedBooks(userId));

    }


    public Result<int> AddBook(string title, string author, string genre, string content)
    {
        if (!IsCurrentUserAdmin())
            return Result<int>.Error("Admin must be logged in to make this operation");

        try
        {
            int bookId = _bookRepository.AddBook(title, author, genre);

            Task<bool> addedContentTask = _contentRepository.AddBookContent(bookId, content);
            addedContentTask.Wait();
            if (addedContentTask.Result == false)
            {
                return Result<int>.Error("Failed to add book content");
            }
            else
            {
                return Result<int>.Success(bookId);
            }
        }
        catch (Exception e)
        {
            return Result<int>.Error(e.Message);

        }
    }

    public Result<string> UpdateBookMetadata(int bookId, string title, string author, string genre)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        BookEntity? book = _bookRepository.GetBookById(bookId);
        if (book == null)
            return Result<string>.Error("Book not found");



        try
        {
            bool updatedBook = _bookRepository.UpdateBook(bookId, title, author, genre);


            if (updatedBook)
            {
                return Result<string>.Success("Book's meta data updated successfully");

            }
            else
            {
                return Result<string>.Error($"Failed to update book's meta data");

            }

        }
        catch (Exception e)
        {
            return Result<string>.Error($"Failed to update book's meta data, Error {e}");

        }
    }
    public Result<string> UpdateBookContent(int bookId, string content)
    {

        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        BookEntity? book = _bookRepository.GetBookById(bookId);
        if (book == null)
            return Result<string>.Error("Book not found");



        try
        {
            Task<bool> bookHasContentTask = _contentRepository.BookHasContent(bookId);
            bookHasContentTask.Wait();
            if (!bookHasContentTask.Result)
            {

                Task<bool> addedContentTask = _contentRepository.AddBookContent(bookId, content);
                addedContentTask.Wait();
                if (addedContentTask.Result == false)
                {
                    return Result<string>.Error("Failed to update and add book content");
                }
                else
                {
                    return Result<string>.Success("Book's content updated successfully");
                }
            }
            else
            {

                Task<bool> updatedContentTask = _contentRepository.UpdateBookContent(bookId, content);
                updatedContentTask.Wait();


                if (updatedContentTask.Result)
                {
                    return Result<string>.Success("Book's content updated successfully");

                }
                else
                {
                    return Result<string>.Error("Failed to update book's content");

                }
            }

        }
        catch (Exception e)
        {
            return Result<string>.Error($"Failed to update book's content, Error {e}");

        }

    }

    public Result<string> GetBookContent(int bookId)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");

        Task<string?> bookContentTask = _contentRepository.GetBookContent(bookId);
        bookContentTask.Wait();

        string? content = bookContentTask.Result;

        if (content == null)
        {
            return Result<string>.Error("Book content not found");
        }
        else
        {
            return Result<string>.Success(content);
        }

    }

    public Result<string> UpdateInventoryBook(int bookId, int newCopies, int newBorrowPeriod)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");


        if (_inventoryRepository.UpdateBook(bookId, newCopies, newBorrowPeriod))
        {
            return Result<string>.Success("Inventory updated successfully");
        }
        else
        {
            return Result<string>.Error("Failed to update inventory");
        }

    }


    public Result<string> AddBookToInventory(int bookId)
    {
        if (!IsCurrentUserAdmin())
            return Result<string>.Error("Admin must be logged in to make this operation");


        if (_inventoryRepository.AddBookToInventory(bookId, 0, 14))
        {
            return Result<string>.Success("Book added successfully with 0 copies and 14 days borrowing period");
        }
        else
        {
            return Result<string>.Error("Failed to update inventory");
        }

    }

    //Querying the database to validate admin status for security
    private bool IsCurrentUserAdmin()
    {
        if (_authService.CurrentUser == null)
            return false;

        return _userRepository.IsUserAdmin(_authService.CurrentUser!.Id);
    }
}