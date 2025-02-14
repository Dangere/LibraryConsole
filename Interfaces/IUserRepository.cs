using LibraryManagementSystem.Models.Entities;
namespace LibraryManagementSystem.Interfaces;
public interface IUserRepository
{
    public UserEntity? GetUser(string email);
    public UserEntity? GetUserById(int id);
    public UserEntity CreateNewUser(string email, string firstName, string lastName, string? phone, string hashed_password, string salt);

    public List<UserEntity> GetAllUsers(int itemsPerPage, int page);

    public bool UpdateUserPassword(int userId, string newHashedPassword);

    public bool UpdateUserEmail(int userId, string newEmail);

    public bool UpdateUserPhone(int userId, string newPhone);
    public bool UpdateUserFirstAndLastName(int userId, string newFirstName, string newLastName);

    public bool IsUserAdmin(int userId);



}