using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.LibraryInventory;
public class InventoryBooksScreen : ConsoleScreen
{
    private readonly InventoryService _inventoryService;

    // this is factory delegate that takes in an InventoryBookDTO and returns a ViewInventoryBookScreen so we can push it to the stack
    private readonly Func<InventoryBookDTO, ViewInventoryBookScreen> _viewBookFactory;
    public InventoryBooksScreen(ConsoleUI consoleUI, InventoryService inventoryService, Func<InventoryBookDTO, ViewInventoryBookScreen> viewInventoryBookScreenFactory) : base(consoleUI)
    {
        _inventoryService = inventoryService;
        _viewBookFactory = viewInventoryBookScreenFactory;

    }

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
        List<InventoryBookDTO> books = _inventoryService.GetBooksFromInventory(_itemsPerPage, page, true);


        var table = new ConsoleTable("Book Id", "Book Title", "Book Author", "Book Genre", "Copies Available", "Borrowing Period");

        foreach (InventoryBookDTO book in books)
        {
            table.AddRow(book.BookId, book.BookTitle, book.BookAuthor, book.BookGenre, book.CopiesAvailable, book.BorrowingPeriod + " days");

        }

        table.Write(Format.Alternative);


        return books.Count;

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

            //check if the book needing to be viewed exists
            Result<InventoryBookDTO> selectedBooKResult = _inventoryService.GetBookById(inputIndex, true);
            if (selectedBooKResult.IsSuccess)
            {
                //display the individual book viewing screen
                PushScreen(_viewBookFactory(selectedBooKResult.Data!));
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


    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [];
        list.Add(new UIOption("Next page", 1, NextPage));


        if (page > 1)
            list.Add(new UIOption("Previous page", 2, PreviousPage));

        list.AddRange(base.UIElements());
        return list;
    }
}