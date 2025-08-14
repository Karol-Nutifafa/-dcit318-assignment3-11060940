namespace FinanceManagementSystem.Interfaces;
using FinanceManagementSystem.Models;

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}
