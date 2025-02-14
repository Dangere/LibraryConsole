using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.LibraryInventory;
public class ViewInventoryBookScreen : ConsoleScreen
{
    private readonly InventoryBookDTO _bookInventoryDetailsDTO;
    private readonly BorrowService _borrowedBooksService;


    public ViewInventoryBookScreen(ConsoleUI consoleUI, BorrowService borrowService, InventoryBookDTO bookFromInventory) : base(consoleUI)
    {
        _bookInventoryDetailsDTO = bookFromInventory;
        _borrowedBooksService = borrowService;
    }

    public override bool DrawScreen()
    {
        Console.WriteLine("Title: " + _bookInventoryDetailsDTO.BookTitle);
        Console.WriteLine("Author: " + _bookInventoryDetailsDTO.BookAuthor);
        Console.WriteLine("Genre: " + _bookInventoryDetailsDTO.BookGenre);
        Console.WriteLine("Copies Available: " + _bookInventoryDetailsDTO.CopiesAvailable + " copies");
        Console.WriteLine("Borrowing Period: " + _bookInventoryDetailsDTO.BorrowingPeriod + " days");

        Console.WriteLine("");
        DisplayFooter();
        return true;
    }

    private void BorrowBook()
    {
        Result<string> result = _borrowedBooksService.BorrowBook(_bookInventoryDetailsDTO);

        if (result.IsSuccess)
        {
            PopScreen("Book borrowed successfully");

        }
        else
        {
            MessageBlock(result.ErrorMessage!);

        }
    }


    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [
        new LineSpacer(),
        new UIOption("Borrow book", 1,  BorrowBook),
        new UIOption("Return", 0, ()=> PopScreen())
        ];


        return list;
    }
}