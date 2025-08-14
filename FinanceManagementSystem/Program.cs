using FinanceManagementSystem.Models;
using FinanceManagementSystem.Services;

namespace FinanceManagementSystem;

public class FinanceApp
{
    private List<Transaction> _transactions = new();

    private decimal GetDecimalInput(string prompt)
    {
        decimal value;
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Please enter a valid positive number.");
        }
    }

    private string GetStringInput(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    private int GetProcessorChoice()
    {
        Console.WriteLine("\nChoose payment processor:");
        Console.WriteLine("1. Mobile Money");
        Console.WriteLine("2. Bank Transfer");
        Console.WriteLine("3. Crypto Wallet");
        
        int choice;
        while (true)
        {
            Console.Write("Enter your choice (1-3): ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 3)
            {
                return choice;
            }
            Console.WriteLine("Please enter a valid choice (1-3).");
        }
    }

    public void Run()
    {
        Console.WriteLine("=== Finance Management System ===\n");

        // Get account details
        string accountNumber = GetStringInput("Enter account number: ");
        decimal initialBalance = GetDecimalInput("Enter initial balance: $");
        var savingsAccount = new SavingsAccount(accountNumber, initialBalance);

        var mobileProcessor = new MobileMoneyProcessor();
        var bankProcessor = new BankTransferProcessor();
        var cryptoProcessor = new CryptoWalletProcessor();

        bool continueTransactions = true;
        int transactionId = 1;

        while (continueTransactions)
        {
            Console.WriteLine($"\nCurrent Balance: ${savingsAccount.Balance}");
            
            // Get transaction details
            string category = GetStringInput("Enter transaction category: ");
            decimal amount = GetDecimalInput("Enter transaction amount: $");
            
            var transaction = new Transaction(transactionId++, DateTime.Now, amount, category);

            // Choose processor
            int processorChoice = GetProcessorChoice();
            
            switch (processorChoice)
            {
                case 1:
                    mobileProcessor.Process(transaction);
                    break;
                case 2:
                    bankProcessor.Process(transaction);
                    break;
                case 3:
                    cryptoProcessor.Process(transaction);
                    break;
            }

            savingsAccount.ApplyTransaction(transaction);
            _transactions.Add(transaction);

            Console.Write("\nDo you want to make another transaction? (y/n): ");
            continueTransactions = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
        }

        // Display transaction history
        Console.WriteLine("\n=== Transaction History ===");
        foreach (var transaction in _transactions)
        {
            Console.WriteLine($"ID: {transaction.Id}, Date: {transaction.Date}, Category: {transaction.Category}, Amount: ${transaction.Amount}");
        }
        Console.WriteLine($"\nFinal Balance: ${savingsAccount.Balance}");
    }

    public static void Main(string[] args)
    {
        var app = new FinanceApp();
        app.Run();
    }
}
