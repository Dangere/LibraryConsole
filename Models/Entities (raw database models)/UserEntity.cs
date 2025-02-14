namespace LibraryManagementSystem.Models.Entities;
public class UserEntity(int id, string firstName, string lastName, string email, string? phone, decimal balance, string hashedPassword, string salt, bool admin, DateTime creationDate)
{
    public int Id { get => id; }
    public string FirstName { get => firstName; }
    public string LastName { get => lastName; }
    public string Email { get => email; }
    public string? Phone { get => phone; }
    public decimal Balance { get => balance; }
    public string HashedPassword { get => hashedPassword; }
    public string Salt { get => salt; }
    public bool IsAdmin { get => admin; }
    public DateTime CreationDate { get => creationDate; }


    public UserEntity CopyWithNewBalance(decimal newBalance)
    {
        return new UserEntity(Id, FirstName, LastName, Email, Phone, newBalance, HashedPassword, Salt, IsAdmin, CreationDate);

    }

    public UserEntity CopyWith(string? firstName = null, string? lastName = null, string? email = null, string? phone = null, decimal? balance = null)
    {
        return new UserEntity(Id, firstName ?? FirstName, lastName ?? LastName, email ?? Email, phone ?? Phone, balance ?? Balance, HashedPassword, Salt, IsAdmin, CreationDate);
    }
}