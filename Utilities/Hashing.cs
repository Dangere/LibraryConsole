using System.Security.Cryptography;
namespace LibraryManagementSystem.Utilities;
public static class Hashing
{

    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);

        }
        return salt;
    }

    public static string HashPassword(string password, byte[] salt)
    {
        using var Pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);

        byte[] hashBytes = Pbkdf2.GetBytes(32);

        return Convert.ToBase64String(hashBytes);
    }

    // public static void MigrateUsersPasswordsToHashes(string dbConnectionString)
    // {

    //     using (DbConnection dbConnection = new MySqlConnection(dbConnectionString))
    //     {
    //         List<UserEntity> users = dbConnection.Query<UserEntity>("SELECT * FROM users").ToList();

    //         foreach (var user in users)
    //         {
    //             byte[] uniqueSalt = GenerateSalt();

    //             string hash = HashPassword(user.Password.ToString(), uniqueSalt);
    //             string storedSalt = Convert.ToBase64String(uniqueSalt);
    //             int userId = user.Id;

    //             dbConnection.Execute("UPDATE users SET salt = @storedSalt, hashed_password = @hash WHERE id = @userId", new { storedSalt, hash, userId });
    //             // Console.WriteLine(user.Id);
    //         }

    //     }

    // }

}