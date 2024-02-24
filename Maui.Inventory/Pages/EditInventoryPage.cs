using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages;

public class EditInventoryPage : BasePage
{
    #region Private Properties
    private InventoryViewModel _ViewModel => (InventoryViewModel)BindingContext;
    private readonly ILanguageService _LangService;
    private Grid _ContentContainer = new()
    {
        Padding = 16,
        RowDefinitions = Rows.Define(Star, Auto),
        RowSpacing = 16
    };
    private ScrollView _Scroll = new();
    private VerticalStackLayout _ContentLayout = new()
    {
        Spacing = 8
    };
    private readonly Image _BarcodePreview = new() { HeightRequest = 70 };
    private readonly MaterialEntry _Description;
    private readonly MaterialEntry _Barcode;
    private readonly MaterialEntry _Quantity;
    private readonly MaterialEntry _LastEdited;
    private readonly MaterialEntry _CreatedOn;
    private readonly MaterialEntry _Status;
    private readonly MaterialEntry _QtyType;
    private readonly MaterialEntry _Location;
    private readonly FloatingActionButton _Save = new() { FABBackgroundColor = Application.Current.Resources["Primary"] as Color, TextColor = Colors.White, FABStyle = FloatingActionButtonStyle.Extended, };
    private readonly FloatingActionButton _Print = new() { FABBackgroundColor = Application.Current.Resources["Primary"] as Color, TextColor = Colors.White, ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Print, Colors.White), FABStyle = FloatingActionButtonStyle.Regular, };
    private readonly FloatingActionButton _Delete = new() { FABBackgroundColor = Colors.Red, TextColor = Colors.White, ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Delete, Colors.White), FABStyle = FloatingActionButtonStyle.Regular, };
    #endregion

    #region Constructors
    public EditInventoryPage(
        ILanguageService languageService,
        InventoryViewModel inventoryViewModel) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = inventoryViewModel;

        _Description = new(inventoryViewModel.DescriptionModel);
        _Barcode = new(inventoryViewModel.BarcodeModel);
        _Quantity = new(inventoryViewModel.QuantityModel);
        _LastEdited = new(inventoryViewModel.LastEditenOn);
        _CreatedOn = new(inventoryViewModel.CreatedOn);
        _Status = new(inventoryViewModel.StatusModel);
        _QtyType = new(inventoryViewModel.QuantityTypeModel);
        _Location = new(inventoryViewModel.LocationModel);

        _Barcode.IsDisabled = true;
        _LastEdited.IsDisabled = true;
        _CreatedOn.IsDisabled = true;
        _Status.IsDisabled = true;
        _QtyType.IsDisabled = true;
        _Location.IsDisabled = true;

        switch (inventoryViewModel.EditMode)
        {
            case EditMode.Edit:
                Title = _LangService.StringForKey("Edit Inventory");
                _Save.Text = _LangService.StringForKey("SaveChanges");

                _ViewModel.DescriptionModel.Text = _ViewModel.SelectedInventory.Description;
                _ViewModel.BarcodeModel.Text = _ViewModel.SelectedInventory.Barcode;
                _ViewModel.QuantityModel.Text = $"{_ViewModel.SelectedInventory.Quantity}";
                _ViewModel.LastEditenOn.Text = $"{_ViewModel.SelectedInventory.LastEditedOn?.ToString("MM/dd/yyyy") ?? ""}";
                _ViewModel.CreatedOn.Text = $"{_ViewModel.SelectedInventory.CreatedOn?.ToString("MM/dd/yyyy") ?? ""}";
                _ViewModel.QuantityTypeModel.Text = _ViewModel.SelectedInventory.QuantityType;
                _ViewModel.StatusModel.Text = _ViewModel.SelectedInventory.Status;
                _ViewModel.LocationModel.Text = _ViewModel.SelectedInventory.Location;
                PopulateBarcode(_ViewModel.SelectedInventory.Barcode);

                _ContentLayout.Add(_Status);
                _ContentLayout.Add(_Description);
                _ContentLayout.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Auto),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _Quantity.Column(0),
                        _QtyType.Column(1)
                    }
                });
                _ContentLayout.Add(_Location);
                _ContentLayout.Add(_Barcode);
                _ContentLayout.Add(_BarcodePreview);
                _ContentLayout.Add(_LastEdited);
                _ContentLayout.Add(_CreatedOn);

                _ContentContainer.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Auto, Auto, Star),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _Delete.Column(0),
                        _Print.Column(1),
                        _Save.Column(2)
                    }
                }.Row(1));

                break;
            case EditMode.Add:
                Title = _LangService.StringForKey("Add Inventory");
                _Save.Text = _LangService.StringForKey("Add");

                string ticks = $"{DateTime.Now.Ticks}";
                PopulateBarcode(ticks);
                _ViewModel.BarcodeModel.Text = ticks;

                _ContentLayout.Add(_Status);
                _ContentLayout.Add(_Description);
                _ContentLayout.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Auto),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _Quantity.Column(0),
                        _QtyType.Column(1)
                    }
                });
                _ContentLayout.Add(_Location);
                _ContentLayout.Add(_Barcode);
                _ContentLayout.Add(_BarcodePreview);
                _ContentLayout.Add(new BoxView { HeightRequest = 8, Color = Colors.Transparent });

                _ContentContainer.Add(_Save.Row(1));

                break;
        }

        _Scroll.Content = _ContentLayout;
        _ContentContainer.Add(_Scroll.Row(0));

        Content = _ContentContainer;

        _Status.TapGesture(SelectStatus);
        _QtyType.TapGesture(SelectQtyType);
        _Location.TapGesture(SelectLocation);

        _Location.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);
        _QtyType.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);
        _Status.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);

        _Save.Clicked += Save;
        _Delete.Clicked += Delete;
        _Print.Clicked += Print;
    }
    ~EditInventoryPage()
    {
        _Save.Clicked -= Save;
        _Delete.Clicked -= Delete;
        _Print.Clicked -= Print;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();

        var selectedStatus = _ViewModel.StatusVM.SelectedItems.FirstOrDefault();
        var selectedLocation = _ViewModel.LocationsVM.SelectedItems.FirstOrDefault();
        var selectedQtyType = _ViewModel.QuantityTypesViewModel.SelectedItems.FirstOrDefault();
        if (selectedStatus != null)
        {
            if (!string.IsNullOrEmpty(selectedStatus.HeadLine))
            {
                _ViewModel.StatusModel.Text = selectedStatus.HeadLine;
            }
        }

        if (selectedLocation != null)
        {
            if (!string.IsNullOrEmpty(selectedLocation.HeadLine))
            {
                _ViewModel.LocationModel.Text = selectedLocation.HeadLine;
            }
        }

        if (selectedQtyType != null)
        {
            if (!string.IsNullOrEmpty(selectedQtyType.HeadLine))
            {
                _ViewModel.QuantityTypeModel.Text = selectedQtyType.HeadLine;
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void PopulateBarcode(string code)
    {
        await _ViewModel.GenerateBarcode(code);
        if (!string.IsNullOrEmpty(_ViewModel.CurrentBarcodeBase64))
        {
            _BarcodePreview.Source = ImageSource.FromStream(() =>
            {
                return new MemoryStream(Convert.FromBase64String(_ViewModel.CurrentBarcodeBase64));
            });
        }
    }

    private async void SelectLocation()
    {
        _Location.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);
        await Navigation.PushModalAsync(new MaterialSelectPopupPage(_LangService, _ViewModel.LocationsVM));
    }

    private async void SelectQtyType()
    {
        _QtyType.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);
        await Navigation.PushModalAsync(new MaterialSelectPopupPage(_LangService, _ViewModel.QuantityTypesViewModel));
    }

    private async void SelectStatus()
    {
        _Status.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);
        await Navigation.PushModalAsync(new MaterialSelectPopupPage(_LangService, _ViewModel.StatusVM));
    }

    private void Print(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_ViewModel.CurrentBarcodeBase64))
            return;

        PrintUtility.PrintBase64Image(_ViewModel.CurrentBarcodeBase64);
    }

    private async void Delete(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LangService.StringForKey("Deleting"))
            return;

        bool delete = await DisplayAlert(
            _LangService.StringForKey("Delete Inventory"),
            _LangService.StringForKey("Are you sure you want to delete?"),
            _LangService.StringForKey("Yes"),
            _LangService.StringForKey("No"));

        if (!delete)
        {
            return;
        }

        _Save.Text = _LangService.StringForKey("Deleting");
        bool result = await _ViewModel.Delete();
        _Save.Text = _LangService.StringForKey("SaveChanges");

        if (!result)
        {
            GenericErrorOccured();
            return;
        }
        _ViewModel.Clean();
        await Navigation.PopAsync();
    }

    private void GenericErrorOccured()
    {
        DisplayAlert(
            _LangService.StringForKey("Error"),
            _LangService.StringForKey("ErrorOccurred"),
            _LangService.StringForKey("OK"));
    }

    private async void Save(object sender, ClickedEventArgs e)
    {
        bool haveDescription = !string.IsNullOrEmpty(_ViewModel.DescriptionModel.Text);
        bool haveQty = !string.IsNullOrEmpty(_ViewModel.QuantityModel.Text);
        bool haveStatus = !string.IsNullOrEmpty(_ViewModel.StatusModel.Text);
        bool haveQtyType = !string.IsNullOrEmpty(_ViewModel.QuantityTypeModel.Text);
        bool haveLocation = !string.IsNullOrEmpty(_ViewModel.LocationModel.Text);

        if (!haveDescription)
        {
            _Description.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }
        if (!haveQty)
        {
            _Quantity.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }
        if (!haveStatus)
        {
            _Status.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }
        if (!haveQtyType)
        {
            _QtyType.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }
        if (!haveLocation)
        {
            _Location.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }

        if (!haveDescription || !haveQty || !haveStatus || !haveQtyType || !haveLocation)
        {
            return;
        }

        _Save.Text = _LangService.StringForKey("Saving");
        bool result = false;
        switch (_ViewModel.EditMode)
        {
            case EditMode.Add:
                result = await _ViewModel.Insert();
                break;
            case EditMode.Edit:
                result = await _ViewModel.Update();
                break;
        }
        _Save.Text = _LangService.StringForKey("SaveChanges");

        if (!result)
        {
            GenericErrorOccured();
            return;
        }
        _ViewModel.Clean();
        await Navigation.PopAsync();
    }
    #endregion
}
