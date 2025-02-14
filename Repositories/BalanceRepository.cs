
using System.Data;
using Dapper;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem.Repositories;
public class BalanceRepository(string connectionString) : IBalanceRepository
{
    private readonly string _connectionString = connectionString;

    public void AddTransaction(int affectedUserId, int? madeByUserId, decimal balanceChange, decimal remainingBalance)
    {
        using IDbConnection connection = new MySqlConnection(_connectionString);

        string insertQuery = @"INSERT INTO balance_history (
                affected_user_id,
                made_by_user_id,
                remaining_balance,
                balance_change)
                VALUES (@affectedUserId, @madeByUserId, @remainingBalance, @balanceChange);";
        connection.Execute(insertQuery, new { affectedUserId, madeByUserId, remainingBalance, balanceChange });
    }

    public List<BalanceTransactionDTO> GetBalanceHistory(int itemsPerPage, int page)
    {
        using IDbConnection connection = new MySqlConnection(_connectionString);
        string selectQuery = @"SELECT affected_user_id,
                CONCAT(a.first_name, ' ', a.last_name) AS affected_user_full_name,
                made_by_user_id,
                CONCAT(b.first_name, ' ', b.last_name) AS made_by_user_full_name,
                remaining_balance,
                balance_change,
                transaction_date
                FROM balance_history
                INNER JOIN users a ON affected_user_id = a.id
                LEFT JOIN users b ON made_by_user_id = b.id
                ORDER BY transaction_date DESC 
                LIMIT @itemsPerPage OFFSET @offset";

        int offset = (page - 1) * itemsPerPage;

        List<BalanceTransactionDTO> balances = connection.Query<BalanceTransactionDTO>(selectQuery, new { itemsPerPage, offset }).ToList();



        return balances;
    }

    public List<BalanceTransactionDTO> GetBalanceHistoryForUser(int userId)
    {
        using IDbConnection connection = new MySqlConnection(_connectionString);


        string selectQuery = @"SELECT affected_user_id,
                CONCAT(a.first_name, ' ', a.last_name) AS affected_user_full_name,
                made_by_user_id,
                CONCAT(b.first_name, ' ', b.last_name) AS made_by_user_full_name,
                remaining_balance,
                balance_change,
                transaction_date
                FROM balance_history
                INNER JOIN users a ON affected_user_id = a.id
                LEFT JOIN users b ON made_by_user_id = b.id 
                ORDER BY transaction_date DESC
                WHERE affected_user_id = @userId";

        List<BalanceTransactionDTO> balances = connection.Query<BalanceTransactionDTO>(selectQuery, new { userId }).ToList();



        return balances;
    }

    public void UpdateBalance(int userId, decimal newBalance)
    {
        using IDbConnection dbConnection = new MySqlConnection(_connectionString);
        string updateQuery = "UPDATE users SET balance = @newBalance WHERE id = @userId";
        dbConnection.Execute(updateQuery, new { newBalance, userId });
    }
}