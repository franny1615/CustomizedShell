﻿using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class UserPermissions
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }
    [JsonPropertyName("inventoryPermissions")]
    public int InventoryPermissions { get; set; }
}

public enum InventoryPermissions
{
    CanEditDesc     = 1,   // 1 0 0 0 0 0 0 0 0 0 
    CanEditQty      = 2,   // 0 1 0 0 0 0 0 0 0 0
    CanEditQtyType  = 4,   // 0 0 1 0 0 0 0 0 0 0
    CanEditStatus   = 8,   // 0 0 0 1 0 0 0 0 0 0
    CanEditLocation = 16,  // 0 0 0 0 1 0 0 0 0 0
    CanAddInventory = 32,  // 0 0 0 0 0 1 0 0 0 0
    CanAddStatus    = 64,  // 0 0 0 0 0 0 1 0 0 0
    CanAddLocation  = 128, // 0 0 0 0 0 0 0 1 0 0
    CanAddQtyType   = 256, // 0 0 0 0 0 0 0 0 1 0 
}