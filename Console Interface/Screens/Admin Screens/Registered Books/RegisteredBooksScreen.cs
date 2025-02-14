using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;
public class RegisteredBooksScreen(Action<BookEntity> onSelectBook, string screenTitle, bool canAddBooks, ConsoleUI consoleUI, BookService bookService) : ConsoleScreen(consoleUI)
{
    private readonly BookService _bookService = bookService;
    private readonly Action<BookEntity> _onSelectBook = onSelectBook;
    private readonly bool _canAddBooks = canAddBooks;
    private readonly string _screenTitle = screenTitle;
    private int page = 1;
    private readonly int _itemsPerPage = 5;
    private readonly int _maxOptionLength = 1;

    public override bool DrawScreen()
    {
        Console.WriteLine(_screenTitle + "\nPage " + page);
        Console.WriteLine("");

        int displayedItemsCount = DisplayedBooks();
        if (displayedItemsCount == 0 && page > 1)
        {

            page--;
            MessageBlock("You reached the last page");
            return false;

        }

        DisplayFooter();

        return true;

    }

    private int DisplayedBooks()
    {
        List<BookEntity> books = _bookService.GetAllBooks(_itemsPerPage, page);

        var table = new ConsoleTable("Book Id", "Title", "Author Name", "Genre", "Creation Date");

        foreach (BookEntity book in books)
        {
            table.AddRow(book.Id, book.Title, book.Author, book.Genre, DateFormatter.FormatDateAndTime(book.CreationDate));
        }

        table.Write(Format.Alternative);
        return books.Count();
    }
    public override string InputMessage => "Please enter a book id from the list above or enter an option";


    public override void ProcessInput(string input)
    {

        if (int.TryParse(input, out int inputIndex))
        {
            //check if the user is entering an option
            //If ExecuteOption executes it will return
            if (input.Length <= _maxOptionLength)
                if (ExecuteOption(inputIndex))
                    return;

            //check if the book needing to be managed exists
            Result<BookEntity> selectedBookResult = _bookService.GetBookById(inputIndex);
            if (selectedBookResult.IsSuccess)
            {
                //execute callback with the selected book
                _onSelectBook(selectedBookResult.Data!);
                return;
            }
            else
            {
                //display error
                MessageBlock(selectedBookResult.ErrorMessage!);
                return;

            }
        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a valid book id from the list and try again");

    }
    private void NextPage()
    {
        page++;

    }
    private void PreviousPage()
    {
        if (page == 1)
            return;
        page--;
    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [];

        if (_canAddBooks)
        {
            list = [
                new UIOption("Add book", 1, () => PushScreen<CreateRegisteredBookScreen>()),
                new UIOption("Next page", 2, NextPage)
            ];
            if (page > 1)
                list.Add(new UIOption("Previous page", 3, PreviousPage));
        }
        else
        {
            list = [
                new UIOption("Next page", 1, NextPage)
            ];
            if (page > 1)
                list.Add(new UIOption("Previous page", 2, PreviousPage));
        }


        list.AddRange(base.UIElements());
        return list;
    }


}