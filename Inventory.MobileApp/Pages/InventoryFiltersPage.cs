using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Pages.Components;
using Inventory.MobileApp.Services;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class InventoryFiltersPage : PopupPage 
{
	private readonly Grid _ContentLayout = new Grid 
	{
		RowDefinitions = Rows.Define(24, Star, Auto),
		BackgroundColor = UIService.Color("PageColor"),
		RowSpacing = 12,
        Padding = 12,
	};
	private readonly Label _Title = new Label()
		.Text(LanguageService.Instance["Filters"])
		.FontSize(16)
		.Center();
	private readonly EntryLikeButton _SortBy = new EntryLikeButton()
	{
		Text = LanguageService.Instance["Last Inserted"]
	}.Placeholder(LanguageService.Instance["Sort By"]);
	private readonly EntryLikeButton _QuantityType = new EntryLikeButton()
		.Placeholder(LanguageService.Instance["Quantity Type"]);
	private readonly EntryLikeButton _Status = new EntryLikeButton()
		.Placeholder(LanguageService.Instance["Status"]);
	private readonly EntryLikeButton _Location = new EntryLikeButton()
		.Placeholder(LanguageService.Instance["Location"]);
	private readonly Button _Cancel = new Button()
		.Text(LanguageService.Instance["Cancel"])
		.BackgroundColor(Color.FromArgb("#646464"))
		.TextColor(Colors.White);
    private readonly Button _Submit = new Button()
		.Text(LanguageService.Instance["OK"]);
	private readonly Label _Clear = new Label()
		.Text(LanguageService.Instance["Clear"])
		.TextColor(UIService.Color("Primary"))
		.Padding(new Thickness(8, 0, 8, 0))
		.FontSize(20)
		.Bold()
		.Start()
		.Top();
	private InventoryFilters _Filters = new();

	public InventoryFiltersPage(
		InventoryFilters currentFilters,
		Action? cancel,
		Action<InventoryFilters>? applyFilters)
	{
		_Filters = currentFilters;
		SetFilters();

		_ContentLayout.Add(_Title.Row(0));
		_ContentLayout.Add(new ScrollView
		{
			Orientation = ScrollOrientation.Vertical,
			Content = new VerticalStackLayout
			{
				Spacing = 8,
				Children = 
				{
					_SortBy,
					_QuantityType,
					_Status,
					_Location,
					_Clear
				}
			}
		}.Row(1));
		_ContentLayout.Add(new Grid 
		{
			ColumnDefinitions = Columns.Define(Auto, Star),
			ColumnSpacing = 12,
			Children = 
			{
				_Cancel.Column(0),
				_Submit.Column(1),
			}
		}.Row(2));

		PopupStyle = PopupStyle.Center;
		PopupContent = _ContentLayout;

		_Cancel.Clicked += async (s, e) => 
		{ 
			await Navigation.PopModalAsync();
			cancel?.Invoke();
		};
		_Submit.Clicked += async (s, e) => 
		{
			await Navigation.PopModalAsync();
			applyFilters?.Invoke(_Filters);
		};

		_Clear.TapGesture(() => 
		{
			_SortBy.Text = LanguageService.Instance["Last Inserted"];
			_QuantityType.Text = "";
			_Status.Text = "";
			_Location.Text = "";

			_Filters = new InventoryFilters();
		});

		_SortBy.Clicked += SelectSortBy;
		_Location.Clicked += PickLocation;
		_QuantityType.Clicked += PickQuantityType;
		_Status.Clicked += PickStatus;
	}

	private void SetFilters()
	{
		_Status.Text = _Filters.Status;
		_QuantityType.Text = _Filters.QuantityType;
		_Location.Text = _Filters.Location;

		string lastInserted = LanguageService.Instance["Last Inserted"];
		string quantity = LanguageService.Instance["Quantity"];
		string quantityType = LanguageService.Instance["Quantity Type"];
		string barcode = LanguageService.Instance["Barcode"];
		string location = LanguageService.Instance["Location"];
		string status = LanguageService.Instance["Status"];
		string createdDate = LanguageService.Instance["Created"];
		string lastEdited = LanguageService.Instance["Last Edited"];
        switch (_Filters.SortBy)
        {
            case InventorySortBy.None:
				_SortBy.Text = lastInserted;
                break;
            case InventorySortBy.Quantity:
				_SortBy.Text = quantity;
                break;
            case InventorySortBy.QuantityType:
				_SortBy.Text = quantityType;
                break;
            case InventorySortBy.Barcode:
				_SortBy.Text = barcode;
                break;
            case InventorySortBy.Location:
				_SortBy.Text = location;
                break;
            case InventorySortBy.Status:
				_SortBy.Text = status;
                break;
            case InventorySortBy.CreatedDate:
				_SortBy.Text = createdDate;
                break;
            case InventorySortBy.LastEditedDate:
				_SortBy.Text = lastEdited;
                break;
        }
    }

    private async void SelectSortBy(object? sender, EventArgs e)
	{
		string lastInserted = LanguageService.Instance["Last Inserted"];
		string quantity = LanguageService.Instance["Quantity"];
		string quantityType = LanguageService.Instance["Quantity Type"];
		string barcode = LanguageService.Instance["Barcode"];
		string location = LanguageService.Instance["Location"];
		string status = LanguageService.Instance["Status"];
		string createdDate = LanguageService.Instance["Created"];
		string lastEdited = LanguageService.Instance["Last Edited"];
		string cancel = LanguageService.Instance["Cancel"];
		string choice = await DisplayActionSheet(
			LanguageService.Instance["Sort By"],
			cancel,
			null,
			[
				lastInserted,
				quantity,
				quantityType,
				barcode,
				location,
				status,
				createdDate,
				lastEdited,
			]
		);
		
		InventorySortBy result = InventorySortBy.None;
		if (choice == quantity)
			result = InventorySortBy.Quantity;
		else if (choice == quantityType)
			result = InventorySortBy.QuantityType;
		else if (choice == barcode)
			result = InventorySortBy.Barcode;
		else if (choice == location)
			result = InventorySortBy.Location;
		else if (choice == status)
			result = InventorySortBy.Status;
		else if (choice == createdDate)
			result = InventorySortBy.CreatedDate;
		else if (choice == lastEdited)
			result = InventorySortBy.LastEditedDate;

		if (!string.IsNullOrEmpty(choice) && choice != cancel)
		{
			_SortBy.Text = choice;
			_Filters.SortBy = result;
		}
	}

	private void PickQuantityType(object? sender, EventArgs e)
	{
		Navigation.PushModalAsync(PageService.PickQtyType(
			LanguageService.Instance["Pick Quantity Type"],
			picked: (qtyType) => 
			{
				_Filters.QuantityType = qtyType.Description;
				_QuantityType.Text = qtyType.Description;
			},
			canceled: () => { }
		));
	}

	private void PickStatus(object? sender, EventArgs e)
	{
		Navigation.PushModalAsync(PageService.PickStatus(
			LanguageService.Instance["Pick Status"],
			picked: (status) => 
			{
				_Filters.Status = status.Description;
				_Status.Text = status.Description;
			},
			canceled: () => { }
		));
	}

	private void PickLocation(object? sender, EventArgs e)
	{
		Navigation.PushModalAsync(PageService.PickLocation(
			LanguageService.Instance["Pick Location"],
			picked: (location) => 
			{
				_Filters.Location = location.Description;
				_Location.Text = location.Description;
			},
			canceled: () => {}
		));
	}
}

public enum InventorySortBy
{
    None = 0,
    Quantity = 1,
    QuantityType = 2,
    Barcode = 3,
    Location = 4,
    Status = 5,
    CreatedDate = 6,
    LastEditedDate = 7
}

public class InventoryFilters 
{
	public InventorySortBy SortBy { get; set; } = InventorySortBy.None;
	public string QuantityType { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
}