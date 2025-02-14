namespace LibraryManagementSystem.Interfaces;


public interface IContentRepository
{
    public Task<string?> GetBookContent(int id);
    public Task<bool> AddBookContent(int bookId, string content);
    public Task<bool> UpdateBookContent(int bookId, string content);
    public Task<bool> BookHasContent(int id);

}