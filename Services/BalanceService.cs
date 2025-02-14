using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.DTOs;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;
public class BalanceService(IBalanceRepository balanceRepository, AuthService authService)
{

    private readonly IBalanceRepository _balanceRepository = balanceRepository;
    private readonly AuthService _authService = authService;


    public List<BalanceTransactionDTO> GetBalanceHistory(int itemsPerPage, int page)
    {
        return _balanceRepository.GetBalanceHistory(itemsPerPage, page);
    }

    public List<BalanceTransactionDTO> GetBalanceHistoryForUser(int userId)
    {
        return _balanceRepository.GetBalanceHistoryForUser(userId);
    }

    public Result<decimal> AddToCurrentUserBalance(decimal amount)
    {
        UserEntity? currentUser = _authService.CurrentUser;
        if (currentUser == null)
            return Result<decimal>.Error("User must be logged in");


        if (currentUser.Balance < amount)
            return Result<decimal>.Error("Not enough balance");



        _balanceRepository.UpdateBalance(currentUser.Id, currentUser.Balance + amount);
        _balanceRepository.AddTransaction(currentUser.Id, null, amount, currentUser.Balance + amount);

        decimal newBalance = currentUser.Balance - amount;
        _authService.RefetchCurrentUser();

        return Result<decimal>.Success(newBalance);
    }


}