
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Authentication;

public class AuthScreen : ConsoleScreen
{
    private readonly AuthService _authService;


    public AuthScreen(AuthService authService, ConsoleUI consoleUI) : base(consoleUI)
    {
        _authService = authService;

    }

    public override bool DrawScreen()
    {
        // base.DrawScreen();
        Console.WriteLine("Account Authentication");

        if (_authService.CurrentUser != null)
        {
            Console.WriteLine("Currently logged in as: " + _authService.CurrentUser.Email);
            Console.WriteLine("Your balance is: " + _authService.CurrentUser.Balance + "$");


        }

        Console.WriteLine("");

        DisplayFooter();

        return true;

    }
    private void Logout()
    {
        _authService.Logout();

        MessageBlock("You have been logged out");
    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [
            new UIOption("Login with account",1,()=> {PushScreen<LoginScreen>(); }),
            new UIOption("Register new account",2,()=> { PushScreen<RegisterScreen>(); }),
        ];


        if (_authService.CurrentUser != null)
        {
            list.Clear();
            list.Add(new UIOption("Logout", 1, Logout, true));
        }

        list.Add(new LineSpacer());

        list.Add(new UIOption("Return", 0, () => PopScreen()));

        return list;
    }
}