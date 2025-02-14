using LibraryManagementSystem.ConsoleInterface.Elements;
using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.ConsoleInterface.Core;

public class ConsoleScreen(ConsoleUI consoleUI)
{
    protected readonly ConsoleUI _consoleUI = consoleUI;

    public virtual string InputMessage { get; private set; } = "Please enter a number from the list above";

    public virtual bool DrawScreen()
    {
        Console.WriteLine("New console screen");
        DisplayFooter();

        return true;
    }
    public virtual void ProcessInput(string input)
    {
        if (int.TryParse(input, out int optionIndex) == true)
        {
            if (ExecuteOption(optionIndex))
                return;
        }

        // handle invalid input
        Console.WriteLine("Invalid input. Please enter a number from the list and try again");
        Console.ReadLine();
    }

    protected virtual List<IConsoleElement> UIElements() => [new LineSpacer(), new UIOption("Return", 0, () => _consoleUI.PopScreen())];

    protected bool ExecuteOption(int index)
    {
        List<IConsoleElement> elements = UIElements();

        //execute the selected option and return true
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i] is UIOption uiOption)
                if (uiOption.Index == index)
                {
                    Console.Clear();
                    uiOption.Execute();
                    return true;
                }
        }

        return false;
    }

    protected void DisplayFooter()
    {

        //display the element' texts
        foreach (IConsoleElement element in UIElements())
        {
            Console.WriteLine(element.DisplayText());
        }

        Console.WriteLine("");
    }

    protected void PushScreen(ConsoleScreen screen, string? message = null)
    {
        if (message != null)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
        _consoleUI.PushScreen(screen);

    }

    protected void PushScreen<T>(string? message = null) where T : ConsoleScreen
    {
        if (message != null)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
        _consoleUI.PushScreen<T>();

    }

    protected void PopScreen(string? message = null)
    {

        if (message != null)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
        _consoleUI.PopScreen();

    }

    protected void MessageBlock(string message)
    {
        Console.WriteLine(message);
        Console.ReadLine();

    }

}
