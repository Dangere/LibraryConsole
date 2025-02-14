
using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.ConsoleInterface.Elements;
public class UIOption(string title, int index, Action onSelect, bool lineSpacing = false) : IConsoleElement
{
    private readonly string _title = title;
    private readonly int _index = index;
    private readonly Action _onSelect = onSelect;

    public int Index { get => _index; }
    public bool LineSpacing { get => lineSpacing; }

    // public string MenuTitle() => $"{_index}. {_title}";
    public void Execute() => _onSelect.Invoke();

    public bool IsSelectable()
    {
        return true;
    }

    public string DisplayText() => $"{_index}. {_title}";
}