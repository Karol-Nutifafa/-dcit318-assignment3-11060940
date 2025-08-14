using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem;

public class WarehouseManager
{
    public readonly InventoryRepository<ElectronicItem> Electronics;
    public readonly InventoryRepository<GroceryItem> Groceries;
    private int _nextElectronicId = 1;
    private int _nextGroceryId = 1;

    public WarehouseManager()
    {
        Electronics = new InventoryRepository<ElectronicItem>();
        Groceries = new InventoryRepository<GroceryItem>();
    }

    private int GetIntInput(string prompt, int minValue = 0)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && value >= minValue)
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
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    public void AddElectronicItem()
    {
        Console.WriteLine("\n=== Add New Electronic Item ===");
        
        try
        {
            string name = GetStringInput("Enter item name: ");
            int quantity = GetIntInput("Enter quantity: ");
            string brand = GetStringInput("Enter brand name: ");
            int warranty = GetIntInput("Enter warranty period (months): ");

            var item = new ElectronicItem(_nextElectronicId++, name, quantity, brand, warranty);
            Electronics.AddItem(item);
            Console.WriteLine($"\nElectronic item added successfully! ID: {item.Id}");
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }

    public void AddGroceryItem()
    {
        Console.WriteLine("\n=== Add New Grocery Item ===");
        
        try
        {
            string name = GetStringInput("Enter item name: ");
            int quantity = GetIntInput("Enter quantity: ");
            int daysUntilExpiry = GetIntInput("Enter days until expiry: ", 1);

            var item = new GroceryItem(_nextGroceryId++, name, quantity, DateTime.Now.AddDays(daysUntilExpiry));
            Groceries.AddItem(item);
            Console.WriteLine($"\nGrocery item added successfully! ID: {item.Id}");
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }

    public void UpdateItemQuantity<T>(InventoryRepository<T> repo, string itemType) where T : IInventoryItem
    {
        Console.WriteLine($"\n=== Update {itemType} Item Quantity ===");
        
        try
        {
            int id = GetIntInput("Enter item ID: ");
            int newQuantity = GetIntInput("Enter new quantity: ");
            
            repo.UpdateQuantity(id, newQuantity);
            Console.WriteLine("Quantity updated successfully!");
        }
        catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }

    public void RemoveItem<T>(InventoryRepository<T> repo, string itemType) where T : IInventoryItem
    {
        Console.WriteLine($"\n=== Remove {itemType} Item ===");
        
        try
        {
            int id = GetIntInput("Enter item ID to remove: ");
            repo.RemoveItem(id);
            Console.WriteLine("Item removed successfully!");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo, string itemType) where T : IInventoryItem
    {
        Console.WriteLine($"\n=== {itemType} Items ===");
        var items = repo.GetAllItems();
        if (items.Count == 0)
        {
            Console.WriteLine($"No {itemType.ToLower()} items in inventory.");
            return;
        }
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}

class Program
{
    static void DisplayMenu()
    {
        Console.WriteLine("\n=== Warehouse Management System Menu ===");
        Console.WriteLine("1. Add New Electronic Item");
        Console.WriteLine("2. Add New Grocery Item");
        Console.WriteLine("3. Update Electronic Item Quantity");
        Console.WriteLine("4. Update Grocery Item Quantity");
        Console.WriteLine("5. Remove Electronic Item");
        Console.WriteLine("6. Remove Grocery Item");
        Console.WriteLine("7. View All Electronic Items");
        Console.WriteLine("8. View All Grocery Items");
        Console.WriteLine("9. Exit");
        Console.Write("\nEnter your choice (1-9): ");
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("=== Warehouse Management System ===\n");
        var manager = new WarehouseManager();
        
        while (true)
        {
            DisplayMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    manager.AddElectronicItem();
                    break;

                case "2":
                    manager.AddGroceryItem();
                    break;

                case "3":
                    manager.UpdateItemQuantity(manager.Electronics, "Electronic");
                    break;

                case "4":
                    manager.UpdateItemQuantity(manager.Groceries, "Grocery");
                    break;

                case "5":
                    manager.RemoveItem(manager.Electronics, "Electronic");
                    break;

                case "6":
                    manager.RemoveItem(manager.Groceries, "Grocery");
                    break;

                case "7":
                    manager.PrintAllItems(manager.Electronics, "Electronic");
                    break;

                case "8":
                    manager.PrintAllItems(manager.Groceries, "Grocery");
                    break;

                case "9":
                    Console.WriteLine("\nThank you for using the Warehouse Management System!");
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;
            }
        }
    }
}
