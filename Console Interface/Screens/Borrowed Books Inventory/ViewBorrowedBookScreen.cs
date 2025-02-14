
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;
public class ViewBorrowedBookScreen(UserBorrowedBookDTO borrowedBook, BorrowService borrowService, ContentService contentService, Func<string, string, ViewBookContent> viewBookContentFactory, ConsoleUI consoleUI) : ConsoleScreen(consoleUI)
{
    private readonly UserBorrowedBookDTO _borrowedBook = borrowedBook;
    private readonly BorrowService _borrowedBooksService = borrowService;
    private readonly ContentService _contentService = contentService;

    private readonly Func<string, string, ViewBookContent> _viewBookContentFactory = viewBookContentFactory;

    public override bool DrawScreen()
    {
        Console.WriteLine("Viewing: " + _borrowedBook.Title);
        Console.WriteLine("    Author: " + _borrowedBook.Author);
        Console.WriteLine("    Genre: " + _borrowedBook.Genre);
        Console.WriteLine("    Borrowed at: " + DateFormatter.ExtractDateOnly(_borrowedBook.BorrowingDate));
        Console.WriteLine("    Expected return date: " + DateFormatter.ExtractDateOnly(_borrowedBook.ReturnDate));
        Console.WriteLine("    Days left: " + _borrowedBook.DaysLeft + " days");
        Console.WriteLine("    Overdue days: " + _borrowedBook.OverdueDays + " days");
        Console.WriteLine("    Penalty: " + _borrowedBook.Penalty + "$");

        Console.WriteLine("");

        DisplayFooter();
        return true;

    }

    private void ReadBook()
    {

        Task<Result<string>> contentResult = _contentService.GetBookContentForCurrentUser(_borrowedBook.BookId);

        contentResult.Wait();

        if (contentResult.Result.IsSuccess)
        {
            //pushing the view book content screen
            PushScreen(_viewBookContentFactory($"Reading: [{_borrowedBook.Title}]", contentResult.Result.Data!));
        }
        else
        {
            MessageBlock(contentResult.Result.ErrorMessage!);
        }

    }


    private void ReturnBook()
    {

        Result<decimal> result = _borrowedBooksService.ReturnBook(_borrowedBook);

        if (result.IsSuccess)
        {
            if (_borrowedBook.Penalty > 0)
                PopScreen("Book returned successfully and penalty has been paid\nYour remaining balance is " + result.Data + "$");
            else
                PopScreen("Book returned successfully");


        }
        else
        {
            MessageBlock(result.ErrorMessage!);

        }

    }


    protected override List<IConsoleElement> UIElements()
    {

        bool isOverdue = _borrowedBook.OverdueDays > 0;
        List<IConsoleElement> list = [
                new UIOption("Read book", 1,   ReadBook),
                new LineSpacer(),
                new UIOption(isOverdue ? "Pay penalty and return book" : "Return book", 2,  ReturnBook),
                new UIOption("Return", 0,  ()  =>PopScreen())
               ];

        return list;
    }
}