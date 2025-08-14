using InventorySystem.Models;
using InventorySystem.Services;

namespace InventorySystem;

public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;
    private int _nextId = 1;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    private int GetIntInput(string prompt, int minValue = 0)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= minValue)
            {
                return value;
            }
            Console.WriteLine($"Please enter a valid number (minimum {minValue}).");
        }
    }

    private string GetStringInput(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim();
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    public void AddNewItem()
    {
        Console.WriteLine("\n=== Add New Inventory Item ===");
        
        try
        {
            string name = GetStringInput("Enter item name: ");
            int quantity = GetIntInput("Enter quantity: ");

            var item = new InventoryItem(_nextId++, name, quantity, DateTime.Now);
            _logger.Add(item);
            Console.WriteLine($"\nItem added successfully! ID: {item.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError adding item: {ex.Message}");
        }
    }

    public void SaveData()
    {
        try
        {
            Console.WriteLine("\nSaving inventory data to file...");
            _logger.SaveToFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError saving data: {ex.Message}");
        }
    }

    public void LoadData()
    {
        try
        {
            Console.WriteLine("\nLoading inventory data from file...");
            _logger.LoadFromFile();
            
            // Update _nextId to be greater than any existing ID
            var maxId = _logger.GetAll().Max(i => i.Id);
            _nextId = maxId + 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError loading data: {ex.Message}");
        }
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        
        if (items.Count == 0)
        {
            Console.WriteLine("\nNo items in inventory.");
            return;
        }

        Console.WriteLine("\n=== Current Inventory ===");
        foreach (var item in items.OrderBy(i => i.Id))
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, " +
                            $"Quantity: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd HH:mm:ss}");
        }
        Console.WriteLine($"Total Items: {items.Count}");
    }

    public void ClearMemory()
    {
        Console.WriteLine("\nClearing inventory from memory...");
        _logger.Clear();
        _nextId = 1;
    }
}

class Program
{
    static void DisplayMenu()
    {
        Console.WriteLine("\n=== Inventory Management System Menu ===");
        Console.WriteLine("1. Add New Item");
        Console.WriteLine("2. View All Items");
        Console.WriteLine("3. Save to File");
        Console.WriteLine("4. Load from File");
        Console.WriteLine("5. Clear Memory");
        Console.WriteLine("6. Exit");
        Console.Write("\nEnter your choice (1-6): ");
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("=== Inventory Management System ===");

        string dataFile = "inventory.json";
        var app = new InventoryApp(dataFile);

        // Try to load existing data
        try
        {
            app.LoadData();
        }
        catch
        {
            Console.WriteLine("No existing data found. Starting with empty inventory.");
        }

        while (true)
        {
            DisplayMenu();
            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        app.AddNewItem();
                        break;

                    case "2":
                        app.PrintAllItems();
                        break;

                    case "3":
                        app.SaveData();
                        break;

                    case "4":
                        app.LoadData();
                        break;

                    case "5":
                        app.ClearMemory();
                        break;

                    case "6":
                        Console.WriteLine("\nThank you for using the Inventory Management System!");
                        return;

                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.WriteLine("Please try again.");
            }
        }
    }
}
