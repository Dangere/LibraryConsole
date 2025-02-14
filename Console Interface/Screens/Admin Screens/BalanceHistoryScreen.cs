
using ConsoleTables;
using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;

public class BalanceHistoryScreen : ConsoleScreen
{
    private readonly BalanceService _balanceService;

    private int page = 1;
    private readonly int _itemsPerPage = 5;

    public BalanceHistoryScreen(ConsoleUI consoleUI, BalanceService balanceService) : base(consoleUI)
    {
        _balanceService = balanceService;
    }



    public override bool DrawScreen()
    {
        Console.WriteLine("You are viewing the balance history\nPage " + page);

        int displayedItemsCount = DisplayTransactions();
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

    public int DisplayTransactions()
    {
        List<BalanceTransactionDTO> balanceTransactions = _balanceService.GetBalanceHistory(_itemsPerPage, page);

        var table = new ConsoleTable("Affected User", "User Id", "Amount", "Remaining", "Made By Admin", "Admin Id", "Transaction Date");

        foreach (BalanceTransactionDTO transaction in balanceTransactions)
        {
            bool madeBySystem = transaction.MadeByUserId == null;
            table.AddRow(transaction.AffectedUserFullName, transaction.AffectedUserId, transaction.BalanceChange, transaction.RemainingBalance, madeBySystem ? "System" : transaction.MadeByUserFullName, madeBySystem ? "NONE" : transaction.MadeByUserId, DateFormatter.FormatDateAndTime(transaction.TransactionDate));

        }

        table.Write(Format.Alternative);

        return balanceTransactions.Count();

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