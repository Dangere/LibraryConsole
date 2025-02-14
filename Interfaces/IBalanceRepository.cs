using LibraryManagementSystem.Models.DTOs;
namespace LibraryManagementSystem.Interfaces;


public interface IBalanceRepository
{
    public List<BalanceTransactionDTO> GetBalanceHistory(int itemsPerPage, int page);

    public List<BalanceTransactionDTO> GetBalanceHistoryForUser(int userId);

    public void AddTransaction(int affectedUserIdm, int? madeByUserId, decimal balanceChange, decimal remainingBalance);

    public void UpdateBalance(int userId, decimal newBalance);



}