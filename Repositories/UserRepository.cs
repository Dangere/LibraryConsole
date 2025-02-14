using System.Data;
using Dapper;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem.Repositories;
public class UserRepository(string connectionString) : IUserRepository
{
    private readonly string _connectionString = connectionString;

    public UserEntity? GetUser(string email)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string selectQuery = @"SELECT id , first_name, last_name, email, phone, balance, hashed_password, salt, admin, creation_date
        FROM users
        WHERE email = @email";

        var parameters = new { email };

        UserEntity? user = dbConnection.QuerySingleOrDefault<UserEntity>(selectQuery, parameters);

        return user;

    }

    public UserEntity CreateNewUser(string email, string firstName, string lastName, string? phone, string hashedPassword, string salt)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string insertAndRefetchQuery = @"INSERT INTO users (first_name, last_name, email, phone, hashed_password, salt) 
        VALUES (@firstName, @lastName, @email, @phone, @hashedPassword, @salt);

        SELECT id , first_name, last_name, email, phone, balance, hashed_password, salt, admin, creation_date FROM users WHERE email = @email;";

        //Executes the query and expects a result coming from a scalar method such as LAST_INSERT_ID()
        // int newUserId = dbConnection.QuerySingleOrDefault<int>(insertQuery, new { firstName, lastName, email, phone, hashedPassword, salt });



        UserEntity createdUser = dbConnection.QuerySingleOrDefault<UserEntity>(insertAndRefetchQuery, new { firstName, lastName, email, phone, hashedPassword, salt }) ?? throw new Exception("Failed to create user");

        return createdUser;
    }

    public List<UserEntity> GetAllUsers(int itemsPerPage, int page)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string selectQuery = @"SELECT id , first_name, last_name, email, phone, balance, hashed_password, salt, admin, creation_date
        FROM users
        ORDER BY id ASC 
        LIMIT @itemsPerPage OFFSET @offset";

        int offset = (page - 1) * itemsPerPage;

        List<UserEntity> users = dbConnection.Query<UserEntity>(selectQuery, new { itemsPerPage, offset }).ToList();

        return users;
    }

    public UserEntity? GetUserById(int id)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string selectQuery = @"SELECT id , first_name, last_name, email, phone, balance, hashed_password, salt, admin, creation_date
        FROM users
        WHERE id = @id";

        var parameters = new { id };

        UserEntity? user = dbConnection.QuerySingleOrDefault<UserEntity>(selectQuery, parameters);

        return user;
    }


    public bool UpdateUserPassword(int userId, string newHashedPassword)
    {

        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = @"UPDATE users SET hashed_password = @newHashedPassword WHERE id = @userId";

        int rowsAffected = dbConnection.Execute(updateQuery, new { userId, newHashedPassword });

        return rowsAffected > 0;
    }

    public bool UpdateUserEmail(int userId, string newEmail)
    {

        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = @"UPDATE users SET email = @newEmail WHERE id = @userId";

        int rowsAffected = dbConnection.Execute(updateQuery, new { userId, newEmail });

        return rowsAffected > 0;
    }

    public bool UpdateUserPhone(int userId, string newPhone)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = @"UPDATE users SET phone = @newPhone WHERE id = @userId";

        int rowsAffected = dbConnection.Execute(updateQuery, new { userId, newPhone });

        return rowsAffected > 0;
    }

    public bool UpdateUserFirstAndLastName(int userId, string newFirstName, string newLastName)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);

        string updateQuery = @"UPDATE users SET first_name = @newFirstName, last_name = @newLastName WHERE id = @userId";

        int rowsAffected = dbConnection.Execute(updateQuery, new { userId, newFirstName, newLastName });

        return rowsAffected > 0;
    }

    public bool IsUserAdmin(int userId)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT admin FROM users WHERE id = @userId";

        return dbConnection.QueryFirstOrDefault<bool?>(selectQuery, new { userId }) ?? false;

    }


}