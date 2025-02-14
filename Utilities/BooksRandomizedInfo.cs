namespace LibraryManagementSystem.Utilities;
public static class BooksRandomizedInfo
{

    public static string GetRandomizedBookTitle()
    {
        string[] titles = {
        "The Silent Sea",
        "Echoes of Eternity",
        "Winds of Change",
        "Shadows of the Past",
        "Whispers in the Dark",
        "Beneath the Starlit Sky",
        "The Forgotten Kingdom",
        "Chronicles of the Lost",
        "Rising Tides",
        "Fragments of Time"
        };

        return titles[Random.Shared.Next(titles.Length)];
    }

    public static string GetRandomizedBookAuthor()
    {
        string[] authors = {
            "Amelia Harper",
            "Liam Caldwell",
            "Sophia Bennett",
            "Oliver Gray",
            "Emma Hayes",
            "Noah Bennett",
            "Isabella Reed",
            "Ethan Turner",
            "Charlotte Adams",
            "James Walker"
            };

        return authors[Random.Shared.Next(authors.Length)];
    }

    public static string GetRandomizedBookGenre()
    {
        string[] genres = {
            "Science Fiction",
            "Fantasy",
            "Historical Fiction",
            "Thriller",
            "Romance",
            "Mystery",
            "Horror",
            "Adventure",
            "Dystopian",
            "Drama"
            };

        return genres[Random.Shared.Next(genres.Length)];
    }

}