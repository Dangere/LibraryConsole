namespace LibraryManagementSystem.Models.DTOs;
public class BalanceTransactionDTO(int affectedUserId, string affectedUserFullName, int? madeByUserId, string? madeByUserFullName, decimal remainingBalance, decimal balanceChange, DateTime transactionDate)
{

    public int AffectedUserId { get => affectedUserId; }
    public string AffectedUserFullName { get => affectedUserFullName; }
    public int? MadeByUserId { get => madeByUserId; }
    public string? MadeByUserFullName { get => madeByUserFullName; }
    public decimal RemainingBalance { get => remainingBalance; }
    public decimal BalanceChange { get => balanceChange; }
    public DateTime TransactionDate { get => transactionDate; }

}