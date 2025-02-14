using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Authentication;

public class RegisterScreen : ConsoleScreen
{
    private readonly AuthService _authService;

    public RegisterScreen(AuthService authService, ConsoleUI consoleUI) : base(consoleUI)
    {
        _authService = authService;

    }


    private string email = "EMPTY";
    private string password = "EMPTY";
    private string firstName = "EMPTY";
    private string lastName = "EMPTY";
    private string phone = "EMPTY";

    private readonly int _maxOptionLength = 1;

    enum Inputting { None, Email, Password, FirstName, LastName, Phone };
    private Inputting inputting = Inputting.None;

    public override bool DrawScreen()
    {
        Console.WriteLine("Please select a field to enter");

        DisplayFooter();
        return true;

    }
    public override string InputMessage
    {
        get
        {
            return inputting switch
            {
                Inputting.None => "Please enter a number from the list above to edit a field or to go back",
                Inputting.FirstName => "You are now entering your first name",
                Inputting.LastName => "You are now entering your last name",
                Inputting.Email => "You are now entering your email",
                Inputting.Password => "You are now entering your password, min 6 max 16 characters, no special characters allowed",
                Inputting.Phone => "You are now entering your phone",
                _ => "Please enter a number from the list above to edit a field or to go back"

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
            case Inputting.FirstName:
                {
                    if (Validators.ValidateName(input))
                    {
                        firstName = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Inputting.LastName:
                {
                    if (Validators.ValidateName(input))
                    {
                        lastName = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Inputting.Email:
                {
                    if (Validators.ValidateEmail(input))
                    {
                        email = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid email format, please try again");
                    return;

                }
            case Inputting.Password:
                {
                    if (Validators.ValidatePassword(input))
                    {
                        password = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid password format, please try again");
                    return;

                }
            case Inputting.Phone:
                {
                    if (Validators.ValidatePhone(input))
                    {
                        phone = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid phone format, please try again");
                    return;

                }
        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");

    }

    private void Register()
    {
        inputting = Inputting.None;

        if (string.Equals(email, "EMPTY") || string.Equals(password, "EMPTY") || string.Equals(phone, "EMPTY") || string.Equals(firstName, "EMPTY") || string.Equals(lastName, "EMPTY"))
        {
            MessageBlock("Please fill all the fields");
            return;
        }

        Result<UserEntity> result = _authService.RegisterWithEmailAndPassword(email, password, firstName, lastName, phone);

        if (result.IsSuccess)
        {
            PopScreen("Registration successful! returning to previous menu");

        }
        else
        {
            MessageBlock("ERROR: " + result.ErrorMessage);

        }
    }

    protected override List<IConsoleElement> UIElements()
    {

        List<IConsoleElement> list = [
            new UIOption("First Name: " + firstName, 1,() => {  inputting = Inputting.FirstName;  }),
            new UIOption("Last Name: " + lastName, 2,() => {  inputting = Inputting.LastName; }),
            new UIOption("Email: " + email, 3,() => { inputting = Inputting.Email; }),
            new UIOption("Password: " + password, 4,() => {  inputting = Inputting.Password; }),
            new UIOption("Phone: " + phone, 5,() => {  inputting = Inputting.Phone; }),
            new LineSpacer(),
            new UIOption("Register", 6, Register, true),
            new UIOption("Return", 0, () => PopScreen())
        ];
        return list;
    }
}