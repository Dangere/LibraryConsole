
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;
public class ContentService(IContentRepository contentRepository, BorrowService borrowService)
{

    private readonly IContentRepository _contentRepository = contentRepository;
    private readonly BorrowService _borrowService = borrowService;



    public async Task<Result<string>> GetBookContentForCurrentUser(int bookId)
    {
        if (!_borrowService.IsBookBorrowedByCurrentUser(bookId))
        {
            return Result<string>.Error("Unauthorized access");

        }

        string? content = await _contentRepository.GetBookContent(bookId);

        if (content == null)
        {
            return Result<string>.Error("Content not found");
        }
        else
        {
            return Result<string>.Success(content);
        }
    }
}