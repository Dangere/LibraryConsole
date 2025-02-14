using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;
public class InventoryManagementScreen(ConsoleUI consoleUI, InventoryService inventoryService, AdminService adminService, Func<InventoryBookDTO, ManageInventoryBookScreen> manageInventoryBookScreenFactory, Func<Action<BookEntity>, string, bool, RegisteredBooksScreen> registeredBooksScreenFactory) : ConsoleScreen(consoleUI)
{
    private readonly InventoryService _inventoryService = inventoryService;
    private readonly AdminService _adminService = adminService;

    // this is factory delegate that takes in an InventoryBookDTO and returns a ManageInventoryBookScreen so we can push it to the stack
    private readonly Func<InventoryBookDTO, ManageInventoryBookScreen> _manageBookFactory = manageInventoryBookScreenFactory;
    private readonly Func<Action<BookEntity>, string, bool, RegisteredBooksScreen> _registeredBooksScreenFactory = registeredBooksScreenFactory;
    private int page = 1;
    private readonly int _itemsPerPage = 5;
    private readonly int _maxOptionLength = 1;


    public override bool DrawScreen()
    {
        Console.WriteLine("Viewing all the available books in the library");
        Console.WriteLine("");

        int displayedItemsCount = DisplayBooks();
        if (displayedItemsCount == 0 && page > 1)
        {

            page--;
            Console.Clear();
            MessageBlock("You reached the last page");
            return false;


        }

        DisplayFooter();
        return true;

    }

    private int DisplayBooks()
    {
        List<InventoryBookDTO> books = _inventoryService.GetBooksFromInventory(_itemsPerPage, page, false);


        var table = new ConsoleTable("Book Id", "Book Title", "Book Author", "Book Genre", "Copies Available", "Borrowing Period");

        foreach (InventoryBookDTO book in books)
        {
            table.AddRow(book.BookId, book.BookTitle, book.BookAuthor, book.BookGenre, book.CopiesAvailable, book.BorrowingPeriod + " days");

        }

        table.Write(Format.Alternative);


        return books.Count;

    }

    public override string InputMessage => "Please enter a book id from the list above to manage it";

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
            Result<InventoryBookDTO> selectedBooKResult = _inventoryService.GetBookById(inputIndex, false);
            if (selectedBooKResult.IsSuccess)
            {
                //display the individual book managing screen
                PushScreen(_manageBookFactory(selectedBooKResult.Data!));
                return;
            }
            else
            {
                //display error
                MessageBlock(selectedBooKResult.ErrorMessage!);
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

    private void OpenRegisteredBooksScreen()
    {
        void OnBookSelectAction(BookEntity book)
        {
            Result<string> result = _adminService.AddBookToInventory(book.Id);
            if (result.IsSuccess)
                MessageBlock(result.Data!);
            else
                MessageBlock(result.ErrorMessage!);
            PopScreen();
        }

        PushScreen(_registeredBooksScreenFactory(OnBookSelectAction, "Viewing registered books", false));
    }


    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [];

        list.Add(new UIOption("Add book to inventory", 1, OpenRegisteredBooksScreen));

        list.Add(new UIOption("Next page", 2, NextPage));


        if (page > 1)
            list.Add(new UIOption("Previous page", 3, PreviousPage));

        list.AddRange(base.UIElements());
        return list;
    }
}