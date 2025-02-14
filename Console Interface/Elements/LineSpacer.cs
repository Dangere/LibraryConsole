using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.ConsoleInterface.Elements;
public class LineSpacer : IConsoleElement
{
    public bool IsSelectable()
    {
        return false;
    }

    public string DisplayText()
    {
        return "--------------";
    }


}