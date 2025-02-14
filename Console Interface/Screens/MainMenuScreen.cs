
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.ConsoleInterface.Screens.Admin;
using LibraryManagementSystem.ConsoleInterface.Screens.Authentication;
using LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;
using LibraryManagementSystem.ConsoleInterface.Screens.LibraryInventory;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;

public class MainMenuScreen : ConsoleScreen
{
    private readonly AuthService _authService;

    public MainMenuScreen(ConsoleUI consoleUI, AuthService authService) : base(consoleUI)
    {
        _authService = authService;
    }

    public override bool DrawScreen()
    {
        Console.WriteLine("Welcome to the library portal");
        UserEntity? user = _authService.CurrentUser;
        Console.WriteLine(user == null ? "Welcome guest!" : "Welcome back " + user.FirstName + " " + user.LastName);
        Console.WriteLine("");
        DisplayFooter();
        return true;


    }

    private void Exit()
    {
        Console.WriteLine("Goodbye");
        Environment.Exit(0);
    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list = [
        new UIOption("View Library", 1, () => PushScreen<InventoryBooksScreen>()),
        new UIOption("View Borrowed Books", 2, () =>  PushScreen<BorrowedBooksScreen>()),
        new UIOption("Account settings", 3, () =>  PushScreen<AuthScreen>()),

        new LineSpacer(),
        new UIOption("Exit", 0, Exit, true)
        ];

        if (_authService.CurrentUser != null && _authService.CurrentUser.IsAdmin)
        {
            list = [.. list.Prepend(new UIOption("Admin Panel", 10, () => PushScreen<AdminPanelScreen>("Entering admin panel")))];

        }

        return list;
    }
}