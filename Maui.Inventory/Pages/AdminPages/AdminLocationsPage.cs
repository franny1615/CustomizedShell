using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminLocationsPage : BasePage
{
    #region Private Properties
    private AdminLocationsViewModel _ViewModel => (AdminLocationsViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly MaterialList<Models.Location> _Search;
    #endregion

    #region Constructor
    public AdminLocationsPage(
        ILanguageService languageService,
        AdminLocationsViewModel viewModel) : base(languageService)
    {
        BindingContext = viewModel;
        _LangService = languageService;

        Title = _LangService.StringForKey("Locations");

        _Search = new(_LangService.StringForKey("No Locations."), MaterialIcon.Shelves, new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "Description");
            view.SetBinding(MaterialCardView.SupportingTextProperty, "Barcode");
            view.Icon = MaterialIcon.Shelves;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += LocationClicked;

            return view;
        }), viewModel, isEditable: AccessControl.IsLicenseValid);

        Content = _Search;

        _Search.AddItemClicked += AddLocation;
    }
    ~AdminLocationsPage()
    {
        _Search.AddItemClicked -= AddLocation;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.FetchPublic();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void LocationClicked(object sender, EventArgs e)
    {
        if (sender is MaterialCardView cardView && cardView.BindingContext is Models.Location location)
        {
            _ViewModel.SelectedLocation = location;
            _ViewModel.Clean();
            Navigation.PushModalAsync(new AdminEditLocationPage(_LangService, _ViewModel));
        }
    }

    private void AddLocation(object sender, ClickedEventArgs e)
    {
        _ViewModel.SelectedLocation = null;
        _ViewModel.Clean();
        Navigation.PushModalAsync(new AdminEditLocationPage(_LangService, _ViewModel));
    }
    #endregion
}

#region Edit Locations
public class AdminEditLocationPage : PopupPage
{
    #region Private Properties
    private AdminLocationsViewModel _ViewModel => (AdminLocationsViewModel)BindingContext;
    private readonly ILanguageService _LangService;
    private readonly ScrollView _Scroll = new() { VerticalScrollBarVisibility = ScrollBarVisibility.Never };
    private readonly VerticalStackLayout _UserInputHolder = new() { Margin = 0, Padding = 0 };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(50, Star, Auto),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        ColumnSpacing = 8,
        RowSpacing = 8,
        Padding = new Thickness(16, 8, 16, 8)
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
    };
    private readonly MaterialImage _Close = new()
    {
        Icon = MaterialIcon.Close,
        IconSize = 30,
        IconColor = Application.Current.Resources["TextColor"] as Color
    };
    private readonly MaterialEntry _Description;
    private readonly MaterialEntry _Barcode;
    private readonly Image _BarcodeView = new();
    private readonly FloatingActionButton _Save = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended,
    };
    private readonly FloatingActionButton _Print = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Print, Colors.White),
        FABStyle = FloatingActionButtonStyle.Regular,
    };
    private readonly FloatingActionButton _Delete = new()
    {
        FABBackgroundColor = Colors.Red,
        TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Delete, Colors.White),
        FABStyle = FloatingActionButtonStyle.Regular,
    };
    #endregion

    #region Constructor
    public AdminEditLocationPage(
        ILanguageService languageService,
        AdminLocationsViewModel viewModel) : base(languageService)
    {
        BindingContext = viewModel;
        _LangService = languageService;

        _Description = new(viewModel.DescriptionModel);
        _Barcode = new(viewModel.BarcodeModel);

        _Barcode.IsDisabled = true; // should always be readonly, this value is auto populated.

        _Close.TapGesture(() => Navigation.PopModalAsync());

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());

        _UserInputHolder.Add(_Description);
        _UserInputHolder.Add(_Barcode);
        _UserInputHolder.Add(_BarcodeView);
        _Scroll.Content = _UserInputHolder;
        _ContentLayout.Add(_Scroll.Row(1).Column(0).ColumnSpan(3));

        switch (viewModel.EditMode)
        {
            case Models.EditMode.Add:
                _Title.Text = _LangService.StringForKey("Add Location");
                _Save.Text = _LangService.StringForKey("Add");

                _Description.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);

                string ticks = $"{DateTime.Now.Ticks}";
                PopulateBarcode(ticks);
                viewModel.BarcodeModel.Text = ticks;

                _ContentLayout.Add(_Save.Row(2).Column(0).ColumnSpan(3));
                break;
            case Models.EditMode.Edit:
                _Title.Text = _LangService.StringForKey("Edit Location");
                _Save.Text = _LangService.StringForKey("SaveChanges");

                viewModel.DescriptionModel.Text = viewModel.SelectedLocation.Description;
                viewModel.BarcodeModel.Text = viewModel.SelectedLocation.Barcode;
                PopulateBarcode(viewModel.SelectedLocation.Barcode);

                _ContentLayout.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Auto, Auto, Star),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _Delete.Column(0),
                        _Print.Column(1),
                        _Save.Column(2)
                    }
                }.Row(2).Column(0).ColumnSpan(3));
                break;
        }

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;

        _Save.Clicked += Save;
        _Delete.Clicked += Delete;
        _Print.Clicked += Print;
    }
    ~AdminEditLocationPage()
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
            _BarcodeView.Source = ImageSource.FromStream(() =>
            {
                return new MemoryStream(Convert.FromBase64String(_ViewModel.CurrentBarcodeBase64));
            });
        }
    }

    private async void Save(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LangService.StringForKey("Saving"))
            return;

        if (string.IsNullOrEmpty(_ViewModel.DescriptionModel.Text))
        {
            _Description.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
            return;
        }

        _Save.Text = _LangService.StringForKey("Saving");
        bool result = false;
        switch (_ViewModel.EditMode)
        {
            case Models.EditMode.Add:
                result = await _ViewModel.Insert();
                break;
            case Models.EditMode.Edit:
                result = await _ViewModel.Update();
                break;
        }
        _Save.Text = _LangService.StringForKey("SaveChanges");

        if (!result)
        {
            GenericErrorOccured();
            return;
        }
        await Navigation.PopModalAsync();
    }

    private async void Delete(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LangService.StringForKey("Deleting"))
            return;

        bool delete = await DisplayAlert(
            _LangService.StringForKey("Delete Location"),
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
        await Navigation.PopModalAsync();
    }

    private void GenericErrorOccured()
    {
        DisplayAlert(
            _LangService.StringForKey("Error"),
            _LangService.StringForKey("ErrorOccurred"),
            _LangService.StringForKey("OK"));
    }

    private void Print(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_ViewModel.CurrentBarcodeBase64))
            return;

        PrintUtility.PrintBase64Image(_ViewModel.CurrentBarcodeBase64);
    }
    #endregion
}
#endregion