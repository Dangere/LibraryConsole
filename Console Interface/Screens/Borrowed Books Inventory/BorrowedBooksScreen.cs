
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;


public class BorrowedBooksScreen : ConsoleScreen
{
    private readonly BorrowService _borrowedBooksService;
    private readonly Func<UserBorrowedBookDTO, ViewBorrowedBookScreen> _viewBorrowedBookScreenFactory;
    public BorrowedBooksScreen(Func<UserBorrowedBookDTO, ViewBorrowedBookScreen> bookScreenFactory, BorrowService borrowService, ConsoleUI consoleUI) : base(consoleUI)
    {
        _borrowedBooksService = borrowService;
        _viewBorrowedBookScreenFactory = bookScreenFactory;
    }


    public override bool DrawScreen()
    {
        Console.WriteLine("Viewing borrowed books");
        Console.WriteLine("");

        DisplayFooter();
        return true;

    }

    private List<IConsoleElement> BooksUIOptions()
    {
        Result<List<UserBorrowedBookDTO>> booksResult = _borrowedBooksService.GetBorrowedBooksForCurrentUser();

        if (!booksResult.IsSuccess)
        {
            PopScreen(booksResult.ErrorMessage);
            return [];
        }

        List<UserBorrowedBookDTO> books = booksResult.Data!;

        List<IConsoleElement> options = [];
        int index = 1;
        foreach (UserBorrowedBookDTO book in books)
        {
            string title = book.Title + ((book.OverdueDays > 0) ? " [OVERDUE]" : $" [Days left: {book.DaysLeft}]");

            options.Add(new UIOption(title, index, () => _consoleUI.PushScreen(_viewBorrowedBookScreenFactory(book))));
            index++;
        }

        return options;
    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = BooksUIOptions();


        list.AddRange(base.UIElements());
        return list;
    }


}
