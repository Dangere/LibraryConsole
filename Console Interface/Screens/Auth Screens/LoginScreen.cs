using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Authentication;

public class LoginScreen : ConsoleScreen
{
    private readonly AuthService _authService;
    public LoginScreen(AuthService authService, ConsoleUI consoleUI) : base(consoleUI)
    {
        _authService = authService;
    }

    private string email = "EMPTY";
    private string password = "EMPTY";

    private enum Inputting { None, Email, Password };
    private Inputting inputting = Inputting.Email;

    private readonly int _maxOptionLength = 1;



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
                Inputting.Email => "You are now entering your email",
                Inputting.Password => "You are now entering your password",
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

        if (inputting == Inputting.Email)
        {
            bool isValidEmail = Validators.ValidateEmail(input);
            if (isValidEmail)
            {
                email = input;
                inputting = Inputting.None;
                return;


            }
            else
            {

                MessageBlock("Invalid email format, please try again");
                return;

            }

        }

        if (inputting == Inputting.Password)
        {
            bool isValidPassword = Validators.ValidatePassword(input);
            if (isValidPassword)
            {
                password = input;
                inputting = Inputting.None;
                return;
            }
            else
            {

                MessageBlock("Invalid password format, please try again");
                return;

            }

        }


        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");

    }

    private void Login()
    {
        inputting = Inputting.None;

        if (string.Equals(email, "EMPTY") || string.Equals(password, "EMPTY"))
        {
            MessageBlock("Please fill all the fields");
            return;
        }

        //if the fields are filled means they are valid, so we can login
        Result<UserEntity> result = _authService.LoginWithEmailAndPassword(email, password);



        if (result.IsSuccess)
        {
            if (result.Data!.IsAdmin)
            {
                PopScreen("Successfully logged in as Admin!");
            }
            else
            {
                PopScreen("Login successful! returning to previous menu");

            }

        }
        else
        {
            MessageBlock("ERROR: " + result.ErrorMessage);

        }

    }


    protected override List<IConsoleElement> UIElements()
    {

        List<IConsoleElement> list = [
            new UIOption("Email: " + email ,1,()=> {inputting = Inputting.Email;}),
            new UIOption("Password: " + password,2,()=> {inputting = Inputting.Password; }),
            new LineSpacer(),
            new UIOption("Login",3, Login, true),
            new UIOption("Return", 0, () =>PopScreen())
        ];


        return list;
    }

}