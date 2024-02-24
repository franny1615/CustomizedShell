using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Controls;
using SQLite;
using System.Text.Json.Serialization;

namespace Maui.Inventory.Models;

[Table("location")]
public partial class Location : ObservableObject, ISelectItem
{
    [PrimaryKey, Column("_id")]
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [Column("admin_id")]
    [JsonPropertyName("adminId")]
    public int AdminId { get; set; } = -1;

    [Column("description")]
    [JsonPropertyName("description")]
    public string Description
    {
        get => HeadLine;
        set => HeadLine = value;
    }

    [Column("barcode")]
    [JsonPropertyName("barcode")]
    public string Barcode 
    { 
        get => SupportingText; 
        set => SupportingText = value; 
    } 

    [ObservableProperty]
    public string headLine = "";

    [ObservableProperty]
    public string supportingText = "";

    [ObservableProperty]
    public bool isSelected = false;
}
