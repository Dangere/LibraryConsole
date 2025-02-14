using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;


public class ViewBookContent : ConsoleScreen
{

    private readonly string _content;
    private readonly string _title;

    private int page = 1;
    private readonly int _maxCharPerPage = 500;
    private readonly int _maxWordsPerLine = 20;

    private readonly string[] pages = [];

    public ViewBookContent(ConsoleUI consoleUI, string title, string content) : base(consoleUI)
    {
        _content = content;
        _title = title;

        if (content.Contains("Mjd"))
            pages = TextUtils.MjdPages();
        else
            pages = TextUtils.CutTextIntoSegments(content, _maxCharPerPage, _maxWordsPerLine);

        // pages = TextUtils.SegmentNewline(content);

    }


    public override bool DrawScreen()
    {
        if (pages.Length == 0)
        {
            PopScreen("No content found");
            return false;
        }

        Console.WriteLine($"{_title}\nPage {page} of {pages.Length}");
        Console.WriteLine("");
        Console.WriteLine($"{pages[page - 1]}");
        Console.WriteLine("");

        DisplayFooter();
        return true;
    }
    private void NextPage()
    {
        if (page == pages.Length)
        {
            MessageBlock("You reached the last page");
            return;
        }

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

