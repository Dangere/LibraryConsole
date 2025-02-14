
using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;

public class UsersManagementScreen(ConsoleUI consoleUI, AdminService adminService, Func<UserEntity, ViewUserManagementScreen> factory) : ConsoleScreen(consoleUI)
{
    private readonly AdminService _adminServices = adminService;
    private readonly Func<UserEntity, ViewUserManagementScreen> _viewUserManagementScreenFactory = factory;

    private int page = 1;
    private readonly int _itemsPerPage = 5;

    private readonly int _maxOptionLength = 1;


    private int DisplayUsers()
    {
        List<UserEntity> users = _adminServices.GetAllUsers(_itemsPerPage, page);


        var table = new ConsoleTable("User Id", "Full Name", "Email", "Phone", "Balance", "Creation Date", "Is Admin");

        foreach (UserEntity user in users)
        {
            table.AddRow(user.Id, user.FirstName + " " + user.LastName, user.Email, user.Phone, user.Balance, DateFormatter.FormatDateAndTime(user.CreationDate), user.IsAdmin ? "Yes" : "No");

        }

        table.Write(Format.Alternative);


        return users.Count;

    }

    public override bool DrawScreen()
    {
        Console.WriteLine("Users Management Screen\nPage " + page);
        Console.WriteLine("");

        int displayedItemsCount = DisplayUsers();
        if (displayedItemsCount == 0 && page > 1)
        {

            page--;
            MessageBlock("You reached the last page");
            return false;

        }

        DisplayFooter();

        return true;

    }
    public override string InputMessage => "Please enter a user id from the list above or enter an option";

    //Read input to see if it matches a user id and display their info, otherwise see if it matches a UI option and execute it
    public override void ProcessInput(string input)
    {

        if (int.TryParse(input, out int inputIndex))
        {
            //check if the user is entering an option
            //If ExecuteOption executes it will return
            if (input.Length <= _maxOptionLength)
                if (ExecuteOption(inputIndex))
                    return;

            //check if the user needing to be managed exists
            Result<UserEntity> selectedUserResult = _adminServices.GetUserById(inputIndex);
            if (selectedUserResult.IsSuccess)
            {
                //display the individual user management screen
                PushScreen(_viewUserManagementScreenFactory(selectedUserResult.Data!));
                return;
            }
            else
            {
                //display error
                MessageBlock(selectedUserResult.ErrorMessage!);
                return;

            }
        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a valid user id from the list and try again");

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