namespace LibraryManagementSystem.Models.Entities;
public class BookEntity()
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }

    public required string Genre { get; set; }

    public required DateTime CreationDate { get; set; }
}