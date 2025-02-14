using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.ConsoleInterface.Core;
public class ConsoleUI(IServiceProvider serviceProvider)
{
    private readonly List<ConsoleScreen> _stack = [];
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private bool screenPopped = false;


    public void Run()
    {

        while (_stack.Count > 0)
        {
            ConsoleScreen currentScreen = _stack.Last();
            bool renderedScreen = RenderScreen(currentScreen);
            if (!screenPopped && renderedScreen)
                ProcessInput(currentScreen);
        }

    }

    private bool RenderScreen(ConsoleScreen screen)
    {
        Console.Clear();
        screenPopped = false;

        return screen.DrawScreen();

    }
    private void ProcessInput(ConsoleScreen screen)
    {
        Console.WriteLine(screen.InputMessage);
        string input = Console.ReadLine() ?? throw new Exception("Input was null");
        screen.ProcessInput(input);

    }

    //push screen to stack from service provider (DI container)
    public ConsoleUI PushScreen<T>() where T : ConsoleScreen
    {
        ConsoleScreen consoleScreen = _serviceProvider.GetRequiredService<T>();
        _stack.Add(consoleScreen);

        return this;
    }

    //push screen to stack from parameter
    public ConsoleUI PushScreen(ConsoleScreen screen)
    {
        _stack.Add(screen);

        return this;

    }


    public void PopScreen()
    {
        if (_stack.Count > 1)
        {
            screenPopped = true;
            _stack.RemoveAt(_stack.Count - 1);
        }
        else
        {
            Console.WriteLine("No screen to fall back to");
            Console.ReadLine();
        }
    }

}
