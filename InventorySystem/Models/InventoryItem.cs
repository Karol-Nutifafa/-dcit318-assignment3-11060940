using InventorySystem.Interfaces;

namespace InventorySystem.Models;

public record InventoryItem(
    int Id,
    string Name,
    int Quantity,
    DateTime DateAdded
) : IInventoryEntity;
