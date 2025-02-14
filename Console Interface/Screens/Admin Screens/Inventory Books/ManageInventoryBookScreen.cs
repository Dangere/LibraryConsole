using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;
public class ManageInventoryBookScreen : ConsoleScreen
{
    private readonly InventoryBookDTO inventoryBook;
    private readonly AdminService _adminService;
    private readonly BorrowService _borrowService;

    enum Editing { None, Copies, BorrowPeriod };
    private Editing editing = Editing.None;

    private int copiesField = 0;
    private int borrowPeriodField = 0;

    private readonly int _maxOptionLength = 2;



    public ManageInventoryBookScreen(InventoryBookDTO bookFromInventory, ConsoleUI consoleUI, AdminService AdminService, BorrowService borrowService) : base(consoleUI)
    {
        inventoryBook = bookFromInventory;
        _adminService = AdminService;
        _borrowService = borrowService;

        copiesField = inventoryBook.CopiesAvailable;
        borrowPeriodField = inventoryBook.BorrowingPeriod;
    }
    private bool HasDoneEditToCopies => copiesField != inventoryBook.CopiesAvailable;
    private bool HasDoneEditToBorrowingPeriod => borrowPeriodField != inventoryBook.BorrowingPeriod;

    public override bool DrawScreen()
    {


        Console.WriteLine("Currently managing book id: " + inventoryBook.BookId);
        Console.WriteLine("");

        Console.WriteLine("Title: " + inventoryBook.BookTitle);
        Console.WriteLine("Author: " + inventoryBook.BookAuthor);
        Console.WriteLine("Genre: " + inventoryBook.BookGenre);
        Console.WriteLine("");
        Console.WriteLine("Copies Available: " + inventoryBook.CopiesAvailable + " copies" + (HasDoneEditToCopies ? $"  =>  {copiesField} copies" : ""));
        Console.WriteLine("Copies Borrowed: " + _borrowService.CopiesBorrowedOfBook(inventoryBook.BookId) + " copies");
        Console.WriteLine("Borrowing Period: " + inventoryBook.BorrowingPeriod + " days" + (HasDoneEditToBorrowingPeriod ? $"  =>  {borrowPeriodField} days" : ""));
        Console.WriteLine("");
        DisplayFooter();
        return true;
    }

    public override string InputMessage
    {
        get
        {
            return editing switch
            {
                Editing.None => base.InputMessage,
                Editing.Copies => "You are now updating the copies available",
                Editing.BorrowPeriod => "You are now updating the borrowing period",
                _ => base.InputMessage

            };
        }
    }

    public override void ProcessInput(string input)
    {
        //check if the user is entering an option or not

        if (!int.TryParse(input, out int parsedInput) && parsedInput < 0)
        {
            MessageBlock("Invalid input. Please enter a valid number and try again");
            return;
        }

        if (input.Length <= _maxOptionLength)
        {
            if (ExecuteOption(parsedInput))
                return;

        }

        //if the user is not entering an option, fill a field 
        switch (editing)
        {
            case Editing.Copies:
                {
                    copiesField = parsedInput;
                    editing = Editing.None;
                    return;
                }
            case Editing.BorrowPeriod:
                {
                    if (parsedInput < 0 || parsedInput > 90)
                    {
                        MessageBlock("Invalid input. Please enter a number between 1 and 90 and try again");
                        return;
                    }
                    borrowPeriodField = parsedInput;
                    editing = Editing.None;

                    return;
                }

        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");
    }

    private void SaveChangesToInventory()
    {

        Result<string> updateResult = _adminService.UpdateInventoryBook(inventoryBook.BookId, copiesField, borrowPeriodField);

        if (updateResult.IsSuccess)
        {
            MessageBlock("Updated inventory!");

            //updating the prefetched book so the changes reflect in the UI without a requery
            if (HasDoneEditToCopies)
                inventoryBook.CopiesAvailable = copiesField;

            if (HasDoneEditToBorrowingPeriod)
                inventoryBook.BorrowingPeriod = borrowPeriodField;

            editing = Editing.None;
        }
        else
        {
            MessageBlock(updateResult.ErrorMessage!);

        }
    }


    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [
        new UIOption("Change copies available", 1,  ()=> { editing = Editing.Copies; }),
        new UIOption("Change borrowing period", 2,  ()=> {editing = Editing.BorrowPeriod;}),


        ];

        if (HasDoneEditToCopies || HasDoneEditToBorrowingPeriod)
            list.Add(new UIOption("Confirm changes", 3, SaveChangesToInventory));

        list.Add(new LineSpacer());

        list.Add(new UIOption("Return", 0, () => PopScreen()));


        if (editing != Editing.None)
            list.Clear();





        return list;
    }
}