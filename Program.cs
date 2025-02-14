using LibraryManagementSystem.ConsoleInterface.Core;
using LibraryManagementSystem.ConsoleInterface.Screens.Admin;
using LibraryManagementSystem.ConsoleInterface.Screens.Authentication;
using LibraryManagementSystem.ConsoleInterface.Screens.BorrowedBooksInventory;
using LibraryManagementSystem.ConsoleInterface.Screens.LibraryInventory;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem;
class Program
{
    static void Main()
    {
        string ConnectionString = "";
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        //Initialize the database schema, run when setting up the project for the first time
        // DatabaseInitializer.Initialize(ConnectionString);


        ServiceCollection serviceCollection = new();

        //Repositories
        serviceCollection.AddTransient<IBookRepository>(provider => new BookRepository(ConnectionString));
        serviceCollection.AddTransient<IInventoryRepository>(provider => new InventoryRepository(ConnectionString));
        serviceCollection.AddTransient<IBorrowRepository>(provider => new BorrowRepository(ConnectionString));
        serviceCollection.AddTransient<IContentRepository>(provider => new ContentRepository(ConnectionString));
        serviceCollection.AddTransient<IUserRepository>(provider => new UserRepository(ConnectionString));
        serviceCollection.AddTransient<IBalanceRepository>(provider => new BalanceRepository(ConnectionString));

        //Services
        serviceCollection.AddSingleton<BookService>();
        serviceCollection.AddSingleton<InventoryService>();
        serviceCollection.AddSingleton<BorrowService>();
        serviceCollection.AddSingleton<ContentService>();
        serviceCollection.AddSingleton<BalanceService>();
        serviceCollection.AddSingleton<AuthService>();
        serviceCollection.AddSingleton<AdminService>();


        //UI 
        serviceCollection.AddSingleton<ConsoleUI>();
        serviceCollection.AddTransient<MainMenuScreen>();

        //Auth screens
        serviceCollection.AddTransient<AuthScreen>();
        serviceCollection.AddTransient<LoginScreen>();
        serviceCollection.AddTransient<RegisterScreen>();

        //Book screens
        serviceCollection.AddTransient<InventoryBooksScreen>();
        serviceCollection.AddTransient<BorrowedBooksScreen>();

        //Admin screens
        serviceCollection.AddTransient<AdminPanelScreen>();
        serviceCollection.AddTransient<BalanceHistoryScreen>();
        serviceCollection.AddTransient<UsersManagementScreen>();
        serviceCollection.AddTransient<CreateRegisteredBookScreen>();
        serviceCollection.AddTransient<InventoryManagementScreen>();





        //View screens factories
        //we are basically saying that we want to add a function that returns ViewInventoryBookScreen and takes in 
        //InventoryBookDTO as a parameter which is needed for the InventoryBooksScreen class's constructor
        serviceCollection.AddTransient<Func<InventoryBookDTO, ViewInventoryBookScreen>>(provider => bookFromInventory => new ViewInventoryBookScreen(provider.GetRequiredService<ConsoleUI>(), provider.GetRequiredService<BorrowService>(), bookFromInventory));

        serviceCollection.AddTransient<Func<UserBorrowedBookDTO, ViewBorrowedBookScreen>>(provider => borrowedBook => new ViewBorrowedBookScreen(borrowedBook, provider.GetRequiredService<BorrowService>(), provider.GetRequiredService<ContentService>(), provider.GetRequiredService<Func<string, string, ViewBookContent>>(), provider.GetRequiredService<ConsoleUI>()));

        serviceCollection.AddTransient<Func<BookEntity, ManageRegisteredBookScreen>>(provider => registeredBook => new ManageRegisteredBookScreen(registeredBook, provider.GetRequiredService<ConsoleUI>(), provider.GetRequiredService<BookService>(), provider.GetRequiredService<AdminService>(), provider.GetRequiredService<Func<string, string, ViewBookContent>>()));


        //factory method that takes in a method and a string and returns a screen
        serviceCollection.AddTransient<Func<Action<BookEntity>, string, bool, RegisteredBooksScreen>>(provider =>
        {
            return (onBookSelectAction, title, canAddBooks) => new RegisteredBooksScreen(onBookSelectAction, title, canAddBooks, provider.GetRequiredService<ConsoleUI>(), provider.GetRequiredService<BookService>());

        });

        //returns a ViewBookContent
        serviceCollection.AddTransient<Func<string, string, ViewBookContent>>
        (provider => (title, content) => new ViewBookContent(provider.GetRequiredService<ConsoleUI>(), title, content));

        serviceCollection.AddTransient<Func<UserEntity, ViewUserManagementScreen>>(provider => userEntity => new ViewUserManagementScreen(userEntity, provider.GetRequiredService<ConsoleUI>(), provider.GetRequiredService<AdminService>()));
        serviceCollection.AddTransient<Func<InventoryBookDTO, ManageInventoryBookScreen>>(provider => inventoryBook => new ManageInventoryBookScreen(inventoryBook, provider.GetRequiredService<ConsoleUI>(), provider.GetRequiredService<AdminService>(), provider.GetRequiredService<BorrowService>()));



        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.GetRequiredService<ConsoleUI>().PushScreen<MainMenuScreen>().Run();


    }
}
