using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;
public class ManageRegisteredBookScreen : ConsoleScreen
{
    private readonly BookEntity _bookEntity;
    private readonly AdminService _adminService;
    private readonly BookService _bookService;

    private readonly Func<string, string, ViewBookContent> _viewContentScreenFactory;


    public ManageRegisteredBookScreen(BookEntity bookEntity, ConsoleUI consoleUI, BookService bookService, AdminService adminService, Func<string, string, ViewBookContent> viewContentScreenFactory) : base(consoleUI)
    {
        _bookEntity = bookEntity;
        _bookService = bookService;
        _adminService = adminService;

        _viewContentScreenFactory = viewContentScreenFactory;

        titleField = bookEntity.Title;
        authorField = bookEntity.Author;
        genreField = bookEntity.Genre;

        Result<string> contentResult = _adminService.GetBookContent(_bookEntity.Id);

        if (contentResult.IsSuccess)
        {
            originalContentContainer = contentResult.Data!;

        }
        else
        {
            originalContentContainer = "";
            MessageBlock(contentResult.ErrorMessage!);

        }

    }


    private string titleField;
    private string authorField;
    private string genreField;




    private string originalContentContainer;
    private string newContentContainer = "";





    private readonly int _maxOptionLength = 2;

    enum Editing { None, Title, Author, Genre, Content };
    private Editing editing = Editing.None;

    public override bool DrawScreen()
    {
        bool editMode = editing != Editing.None;
        Console.WriteLine((editMode ? "Editing book id: " : "Viewing book id: ") + _bookEntity.Id);
        Console.WriteLine("Title: " + _bookEntity.Title + (editMode ? $"  =>  {titleField} " : ""));
        Console.WriteLine("Author: " + _bookEntity.Author + (editMode ? $"  =>  {authorField} " : ""));
        Console.WriteLine("Genre: " + _bookEntity.Genre + (editMode ? $"  =>  {genreField} " : ""));
        Console.WriteLine("Content: " + (MadeChangesToContent && editMode ? TextUtils.CropString(newContentContainer, 40) + " [edited] " : TextUtils.CropString(originalContentContainer, 40)));

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
                Editing.Title => "You are now entering the new book title",
                Editing.Author => "You are now entering the new book author",
                Editing.Genre => "You are now selecting the new book genre",
                Editing.Content => "You are now entering the content of the book",


                _ => base.InputMessage

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
        switch (editing)
        {
            case Editing.Title:
                {
                    if (Validators.ValidateBookTitle(input))
                    {
                        titleField = input;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Editing.Author:
                {
                    if (Validators.ValidateBookAuthor(input))
                    {
                        authorField = input;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Editing.Content:
                {
                    if (input.Trim().Length < 50)
                    {
                        MessageBlock("Please enter at least 50 characters");
                        return;
                    }

                    newContentContainer = input.Trim();
                    OpenContentScreen(input.Trim(), "You have entered the following content");
                    return;

                }
        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");
    }

    private void OpenContentScreen(string content, string title)
    {
        PushScreen(_viewContentScreenFactory(title, content));
    }


    private void ConfirmEdit()
    {
        if (!MadeChangesToMeta && !MadeChangesToContent)
        {
            MessageBlock("No changes were made to the book");
            return;
        }

        if (!ConfirmPopup("Are you sure you want to save these changes? [y/n]"))
        {
            MessageBlock("Changes were not saved");
            return;
        }
        editing = Editing.None;


        string resultMessage = "";


        if (MadeChangesToMeta)
        {
            Result<string> metaUpdateResult = _adminService.UpdateBookMetadata(_bookEntity.Id, titleField, authorField, genreField);

            // MessageBlock(metaUpdateResult.Data! + "\n" + contentUpdateResult.Data!);
            if (metaUpdateResult.IsSuccess)
            {
                //update the prefetched book so the changes reflect in the UI without a requery
                _bookEntity.Title = titleField;
                _bookEntity.Author = authorField;
                _bookEntity.Genre = genreField;
                resultMessage += metaUpdateResult.Data! + "\n";
            }
            else
            {
                resultMessage += metaUpdateResult.ErrorMessage! + "\n";

            }
        }

        if (MadeChangesToContent)
        {
            Result<string> contentUpdateResult = _adminService.UpdateBookContent(_bookEntity.Id, newContentContainer);
            if (contentUpdateResult.IsSuccess)
            {
                originalContentContainer = newContentContainer;
                newContentContainer = "";
                resultMessage += contentUpdateResult.Data! + "\n";

            }
            else
            {
                resultMessage += contentUpdateResult.ErrorMessage! + "\n";

            }
        }

        MessageBlock(resultMessage);


    }

    private List<IConsoleElement> EditUIOptions()
    {
        List<IConsoleElement> list =
        [new UIOption("Edit title", 1, () => { editing = Editing.Title; }),
        new UIOption("Edit author", 2, () => { editing = Editing.Author; }),
        new UIOption("Edit genre", 3, () => { editing = Editing.Genre;}),
        new UIOption("Edit content", 4, () => { editing = Editing.Content;}),

        new LineSpacer(),
        new UIOption("View original content", 5, () => { OpenContentScreen(originalContentContainer,"Current unedited book content");}),

        ];


        if (MadeChangesToContent)
        {
            list.AddRange([
            new UIOption("View edited content", 6, () => { OpenContentScreen(newContentContainer,"You have entered the following content");}),
            new LineSpacer(),
            new UIOption("Confirm edit" , 7, ConfirmEdit ),
            ]);
        }
        else
        {
            list.AddRange([
            new LineSpacer(),
            new UIOption("Confirm edit", 6, ConfirmEdit)
            ]);
        }

        list.Add(new UIOption("Return to view", 0, () => { editing = Editing.None; }));

        return list;
    }
    private List<IConsoleElement> GenreUIOptions()
    {

        List<IConsoleElement> list = [];
        string[] genreTypes = _bookService.GetBookGenres();
        for (int i = 0; i < genreTypes.Length; i++)
        {
            int index = i + 1;
            string genreType = genreTypes[i];
            list.Add(new UIOption(genreType, index, () => { genreField = genreType; editing = Editing.Title; }));
        }

        list = list.Append(new LineSpacer()).ToList();
        list = list.Append(new UIOption("Return to edit menu", 0, () => { editing = Editing.Title; })).ToList();

        return list;
    }


    protected override List<IConsoleElement> UIElements()
    {
        if (editing == Editing.Genre)
        {
            return GenreUIOptions();
        }

        if (editing != Editing.None)
        {
            return EditUIOptions();
        }

        List<IConsoleElement> list = [
            new UIOption("Edit book" , 1,() => { editing = Editing.Title; }),
            new UIOption("View content", 2, () => { OpenContentScreen(originalContentContainer,"Book content");}),

            new LineSpacer(),

            new UIOption("Return", 0, () => PopScreen())
        ];


        return list;
    }

    bool MadeChangesToContent => !string.IsNullOrEmpty(newContentContainer);
    bool MadeChangesToMeta => titleField != _bookEntity.Title || authorField != _bookEntity.Author || genreField != _bookEntity.Genre;


    bool ConfirmPopup(string message)
    {
        Console.WriteLine(message);
        string input = Console.ReadLine()!;

        if (input.ToLower() == "y" || input.ToLower() == "yes")
            return true;

        return false;
    }
}