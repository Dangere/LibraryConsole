
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;

public class AdminPanelScreen(ConsoleUI consoleUI, Func<Action<BookEntity>, string, bool, RegisteredBooksScreen> registeredBooksScreenFactory, Func<BookEntity, ManageRegisteredBookScreen> manageRegisteredBookScreenFactory) : ConsoleScreen(consoleUI)
{
    private readonly Func<Action<BookEntity>, string, bool, RegisteredBooksScreen> _registeredBooksScreenFactory = registeredBooksScreenFactory;
    private readonly Func<BookEntity, ManageRegisteredBookScreen> _manageRegisteredBookScreenFactory = manageRegisteredBookScreenFactory;
    public override bool DrawScreen()
    {
        Console.WriteLine("Welcome to the admin panel");
        Console.WriteLine("");
        DisplayFooter();

        return true;


    }

    protected override List<IConsoleElement> UIElements()
    {
        List<IConsoleElement> list =
        [
            new UIOption("Manage Users", 1, () => PushScreen<UsersManagementScreen>()),
            new UIOption("Manage the library's inventory", 2, () => PushScreen<InventoryManagementScreen>()),
            new UIOption("Manage registered books", 3, () => PushScreen(_registeredBooksScreenFactory((book) =>
            {
                PushScreen(_manageRegisteredBookScreenFactory(book));
            }, "Managing registered books", true))),
            new UIOption("View balance history", 4, () => PushScreen<BalanceHistoryScreen>()),

        ];

        list.AddRange(base.UIElements());
        return list;
    }



}