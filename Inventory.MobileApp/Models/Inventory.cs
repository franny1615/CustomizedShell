using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class Inventory : INotifyPropertyChanged
{
    private int _Id = -1;
    [JsonPropertyName("id")]
    public int Id 
    { 
        get => _Id; 
        set
        {
            _Id = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
        }
    }
    
    private int _CompanyId = -1;
    [JsonPropertyName("companyId")]
    public int CompanyId 
    { 
        get => _CompanyId;
        set
        { 
            _CompanyId = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CompanyId)));
        } 
    }

    private string _Description = string.Empty;
    [JsonPropertyName("description")]
    public string Description 
    { 
        get => _Description;
        set
        { 
            _Description = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        } 
    }

    private string _Status = string.Empty;
    [JsonPropertyName("status")]
    public string Status 
    { 
        get => _Status; 
        set
        {
            _Status = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    private int _Quantity = -1;
    [JsonPropertyName("quantity")]
    public int Quantity 
    { 
        get => _Quantity; 
        set
        {
            _Quantity = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
        }
    }

    private string _QuantityType = string.Empty;
    [JsonPropertyName("quantityType")]
    public string QuantityType 
    { 
        get => _QuantityType; 
        set
        {
            _QuantityType = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuantityType)));
        }
    }

    private string _Barcode = string.Empty;
    [JsonPropertyName("barcode")]
    public string Barcode 
    { 
        get => _Barcode; 
        set
        {
            _Barcode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Barcode)));
        }
    }

    private string _Location = string.Empty;
    [JsonPropertyName("location")]
    public string Location 
    { 
        get => _Location; 
        set
        {
            _Location = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
        }
    }

    private DateTime? _LastEditedOn = null;
    [JsonPropertyName("lastEditedOn")]
    public DateTime? LastEditedOn 
    { 
        get => _LastEditedOn;
        set
        { 
            _LastEditedOn = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastEditedOn)));
        } 
    }

    [JsonPropertyName("createdOn")]
    public DateTime? CreatedOn { get; set; } = null;
    [JsonPropertyName("qtyTypeID")]
    public int QtyTypeID { get; set; } = -1;
    [JsonPropertyName("locationID")]
    public int LocationID { get; set; } = -1;
    [JsonPropertyName("statusID")]
    public int StatusID { get; set; } = -1;

    public event PropertyChangedEventHandler? PropertyChanged;
}