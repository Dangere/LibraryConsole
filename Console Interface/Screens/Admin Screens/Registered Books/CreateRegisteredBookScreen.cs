using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.ConsoleInterface.Screens.Admin;
public class CreateRegisteredBookScreen(ConsoleUI consoleUI, AdminService adminService, BookService bookService, Func<string, string, ViewBookContent> viewContentScreenFactory) : ConsoleScreen(consoleUI)
{
    private readonly AdminService _adminService = adminService;
    private readonly BookService _bookService = bookService;
    private readonly Func<string, string, ViewBookContent> _viewContentScreenFactory = viewContentScreenFactory;
    private string titleField = "EMPTY";
    private string authorField = "EMPTY";
    private string genreField = "EMPTY";
    private string contentField = "EMPTY";

    private string contentContainer = "";



    private readonly int _maxOptionLength = 2;

    enum Inputting { None, Title, Author, Genre, Content };
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
                Inputting.Title => "You are now entering the book title",
                Inputting.Author => "You are now entering the book author",
                Inputting.Genre => "You are now selecting the book genre",
                Inputting.Content => "You are now entering the content of the book",

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
            case Inputting.Title:
                {
                    if (Validators.ValidateBookTitle(input))
                    {
                        titleField = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Inputting.Author:
                {
                    if (Validators.ValidateBookAuthor(input))
                    {
                        authorField = input;
                        inputting = Inputting.None;

                    }
                    else
                        MessageBlock("Invalid name format, please try again");
                    return;

                }
            case Inputting.Content:
                {

                    if (input.Trim().Length < 50)
                    {
                        MessageBlock("Please enter at least 50 characters");
                        return;
                    }

                    contentContainer = input.Trim();
                    contentField = TextUtils.CropString(input.Trim(), 40);
                    inputting = Inputting.None;
                    OpenContentScreen(input.Trim());
                    return;


                }
        }

        //handle invalid input
        MessageBlock("Invalid input. Please enter a number from the list and try again");

    }


    private void AddBook()
    {
        if (titleField == "EMPTY" || authorField == "EMPTY" || genreField == "EMPTY" || contentField == "EMPTY")
        {
            MessageBlock("Please fill all the fields");
            return;

        }
        Result<int> addResult = _adminService.AddBook(titleField, authorField, genreField, contentContainer);
        if (addResult.IsSuccess)
        {
            MessageBlock("Book added successfully with id " + addResult.Data);
            PopScreen();
        }
        else
        {
            MessageBlock(addResult.ErrorMessage!);
        }
    }

    private void OpenContentScreen(string content)
    {
        PushScreen(_viewContentScreenFactory("You have entered the following content", content));
    }


    private List<IConsoleElement> GenreUIOptions()
    {

        List<IConsoleElement> list = [];
        string[] genreTypes = _bookService.GetBookGenres();
        for (int i = 0; i < genreTypes.Length; i++)
        {
            int index = i + 1;
            string genreType = genreTypes[i];
            list.Add(new UIOption(genreType, index, () => { genreField = genreType; inputting = Inputting.None; }));
        }

        list = list.Append(new LineSpacer()).ToList();
        list = list.Append(new UIOption("Return", 0, () => { inputting = Inputting.None; })).ToList();

        return list;
    }

    protected override List<IConsoleElement> UIElements()
    {

        List<IConsoleElement> list = [
            new UIOption("New Book's Title: " + titleField, 1,() => {  inputting = Inputting.Title;  }),
            new UIOption("New Book Author: " + authorField, 2,() => {  inputting = Inputting.Author; }),
            new UIOption("New Book Genre: " + genreField, 3,() => { inputting = Inputting.Genre; }),
            new UIOption("New Book Content: " +  contentField, 4,() => { inputting = Inputting.Content; }),
        ];

        if (!string.IsNullOrEmpty(contentContainer))
            list.Add(

                new UIOption("View full book content", 5, () => { OpenContentScreen(contentContainer); })
            );


        list.AddRange([
            new LineSpacer(),
            new UIOption("Add book", 6, AddBook, true),
            new UIOption("Return", 0, () => PopScreen())]
            );

        if (inputting == Inputting.Genre)
        {
            list.Clear();
            list.AddRange(GenreUIOptions());
        }


        return list;
    }
}