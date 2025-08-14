using System.Text.Json;
using InventorySystem.Interfaces;

namespace InventorySystem.Services;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log;
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _log = new List<T>();
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log.ToList();
    }

    public void Clear()
    {
        _log.Clear();
    }

    public void SaveToFile()
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_log, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            using var writer = new StreamWriter(_filePath);
            writer.Write(jsonString);
            
            Console.WriteLine($"Data successfully saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
            throw;
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"No existing data file found at {_filePath}");
                return;
            }

            using var reader = new StreamReader(_filePath);
            string jsonString = reader.ReadToEnd();
            
            var items = JsonSerializer.Deserialize<List<T>>(jsonString);
            if (items != null)
            {
                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"Successfully loaded {items.Count} items from {_filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            throw;
        }
    }
}
