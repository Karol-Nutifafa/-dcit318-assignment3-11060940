using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;

namespace WarehouseManagementSystem.Repositories;

public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists in inventory.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T? item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
        }
    }

    public List<T> GetAllItems()
    {
        return _items.Values.ToList();
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException($"Quantity cannot be negative. Received: {newQuantity}");
        }

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}
