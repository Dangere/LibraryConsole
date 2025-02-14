using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;

public class ViewUserManagementScreen(UserEntity user, ConsoleUI consoleUI, AdminService adminService) : ConsoleScreen(consoleUI)
{
    private UserEntity user = user;
    private readonly AdminService _adminService = adminService;


    private enum Inputting { Option, Balance, Password, Email, Name, Phone };

    private Inputting inputting = Inputting.Option;
    private readonly int _maxOptionLength = 1;


    public override bool DrawScreen()
    {
        Console.WriteLine("Currently managing user id: " + user.Id);

        Console.WriteLine("");
        Console.WriteLine($"User Id: {user.Id} |Full Name: {user.FirstName} {user.LastName} |Email: {user.Email} |Phone: {user.Phone}");

        Console.WriteLine($"Balance: {user.Balance}$ |Creation Date: {DateFormatter.FormatDateAndTime(user.CreationDate)} |Is Admin: {(user.IsAdmin ? "Yes" : "No")}");
        Console.WriteLine("");


        Result<List<UserBorrowedBookDTO>> borrowedBooksResult = _adminService.GetBorrowedBooksForUser(user.Id);

        if (!borrowedBooksResult.IsSuccess)
        {
            PopScreen(borrowedBooksResult.ErrorMessage);
            return false;
        }

        List<UserBorrowedBookDTO> borrowedBooks = borrowedBooksResult.Data!;

        Console.WriteLine("Borrowed books: " + borrowedBooks.Count + " book(s)");

        ConsoleTable booksTable = new("Is Overdue", "Book Id", "Title", "Borrow Date", "Return Date", "Days Left", "Overdue Days", "Penalty");
        foreach (UserBorrowedBookDTO book in borrowedBooks)
        {
            booksTable.AddRow(book.OverdueDays > 0 ? "Yes" : "No", book.BookId, book.Title, DateFormatter.ExtractDateOnly(book.BorrowingDate), DateFormatter.ExtractDateOnly(book.ReturnDate), book.DaysLeft, book.OverdueDays, book.Penalty);
        }

        if (borrowedBooks.Count > 0)
            booksTable.Write(Format.MarkDown);


        DisplayFooter();
        return true;


    }


    public override string InputMessage
    {
        get
        {
            return inputting switch
            {
                Inputting.Option => "Please enter a number from the list above to edit a field or to go back",
                Inputting.Balance => "You are now entering a balance to be added (+) or subtracted (-)",
                Inputting.Password => "You are now entering the new password",
                Inputting.Email => "You are now entering the new email",
                Inputting.Name => "You are now entering the new first and last name and separated by a space",
                Inputting.Phone => "You are now entering the new phone",
                _ => "Please enter a number from the list above to edit a field or to go back",

            };

        }
    }
    public override void ProcessInput(string input)
    {


        //check if the user is entering an option or not
        if (input.Length <= _maxOptionLength && int.TryParse(input, out int optionIndex))
        {
            if (ExecuteOption(optionIndex))
                return;

        }


        //if the user is not entering an option, check if the input is valid to fill a field 
        switch (inputting)
        {

            case Inputting.Balance:
                {
                    HandleBalanceInput(input);

                    return;
                }

            case Inputting.Password:
                {
                    HandlePasswordInput(input);
                    return;

                }
            case Inputting.Email:
                {
                    HandleEmailInput(input);
                    return;

                }
            case Inputting.Name:
                {
                    HandleNameInput(input);
                    return;

                }
            case Inputting.Phone:
                {
                    HandlePhoneInput(input);
                    return;

                }

        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");
    }

    private void HandleNameInput(string input)
    {
        string[] namesArray = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (namesArray.Length != 2)
        {
            MessageBlock("Please enter your first and last name separated by a space");
            return;
        }


        Result<string> nameChangeResult = _adminService.UpdateUserFirstAndLastName(user.Id, namesArray[0], namesArray[1]);

        if (nameChangeResult.IsSuccess)
        {
            MessageBlock("Updated user's first and last name!");
            user = user.CopyWith(firstName: namesArray[0], lastName: namesArray[1]);
            inputting = Inputting.Option;
        }
        else
        {
            MessageBlock(nameChangeResult.ErrorMessage!);
        }

    }

    private void HandleBalanceInput(string inputtedBalance)
    {

        if (!decimal.TryParse(inputtedBalance, out decimal addedBalance))
        {
            MessageBlock("Invalid input. Please enter a balance to be added (+) or subtracted (-)");
            return;
        }

        Result<decimal> balanceChangeResult = _adminService.AddUserBalance(user.Id, user.Balance, addedBalance);

        if (balanceChangeResult.IsSuccess)
        {

            MessageBlock((addedBalance > 0 ? "Added " : "Subtracted ") + balanceChangeResult.Data + " to the user balance!");
            user = user.CopyWithNewBalance(user.Balance + addedBalance);
            inputting = Inputting.Option;
        }
        else
        {
            MessageBlock(balanceChangeResult.ErrorMessage!);
        }


    }


    private void HandleEmailInput(string newEmail)
    {

        Result<string> updateResult = _adminService.UpdateUserEmail(user.Id, user.Email, newEmail);

        if (updateResult.IsSuccess)
        {
            MessageBlock("Updated user's email!");
            user = user.CopyWith(email: newEmail);
            inputting = Inputting.Option;
        }
        else
        {
            MessageBlock(updateResult.ErrorMessage!);

        }


    }

    private void HandlePhoneInput(string newPhone)
    {

        Result<string> updateResult = _adminService.UpdateUserPhone(user.Id, user.Phone, newPhone);

        if (updateResult.IsSuccess)
        {
            MessageBlock("Updated user's password!");
            user = user.CopyWith(phone: newPhone);

            inputting = Inputting.Option;
        }
        else
        {
            MessageBlock(updateResult.ErrorMessage!);

        }


    }


    private void HandlePasswordInput(string newPassword)
    {

        Result<string> updateResult = _adminService.UpdateUserPassword(user.Id, user.Salt, user.HashedPassword, newPassword);

        if (updateResult.IsSuccess)
        {
            MessageBlock("Updated user's password!");
            inputting = Inputting.Option;
        }
        else
        {
            MessageBlock(updateResult.ErrorMessage!);

        }



    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [
            new UIOption("Change balance", 1, () => {inputting = Inputting.Balance;  }),
            new UIOption("Change email", 2, () =>  {inputting = Inputting.Email;  }),
            new UIOption("Change first and last name", 3, () => {inputting = Inputting.Name;  }),
            new UIOption("Change phone", 4, () => {inputting = Inputting.Phone ;  }),
            new UIOption("Change password", 5, () => {inputting = Inputting.Password ;  })

        ];

        list.AddRange(base.UIElements());


        //making sure if we are inputting balance we only provide the default return option with the index of 0
        //so the user can enter a balance of all numbers but 0
        if (inputting == Inputting.Balance)
        {
            list = [new UIOption("Return", 0, () => { inputting = Inputting.Option; })];
        }

        //if the user we are trying to manage is an admin we only provide the default return option
        if (user.IsAdmin)
            list = base.UIElements();
        return list;
    }
}