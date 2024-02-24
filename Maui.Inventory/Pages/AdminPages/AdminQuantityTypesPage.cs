using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminQuantityTypesPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private AdminQuantityTypesViewModel _ViewModel => (AdminQuantityTypesViewModel)BindingContext;
    private readonly MaterialList<ISelectItem> _Search;
    #endregion

    #region Constructor
    public AdminQuantityTypesPage(
        ILanguageService languageService,
        AdminQuantityTypesViewModel quantityTypesVM) : base(languageService)
    {
        BindingContext = quantityTypesVM;

        _LangService = languageService;
        Title = _LangService.StringForKey("Quantity Types");

        _Search = new(noItemsText: _LangService.StringForKey("No Quantity Types."), noItemsIcon: MaterialIcon.Video_label, new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "Description");
            view.SupportingText = "";
            view.Icon = MaterialIcon.Video_label;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += AddQuantityType;

            return view;
        }),quantityTypesVM, isEditable: AccessControl.IsLicenseValid);

        Content = _Search;
        _Search.AddItemClicked += AddQuantityType;
    }
    ~AdminQuantityTypesPage()
    {
        _Search.AddItemClicked -= AddQuantityType;
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
    private void AddQuantityType(object sender, EventArgs e)
    {
        if (sender is MaterialCardView card && card.BindingContext is QuantityType qtyType)
        {
            _ViewModel.SelectedQtyType = qtyType;
            _ViewModel.Clean();
            Navigation.PushModalAsync(new AdminEditQuantityTypePage(_LangService, _ViewModel));
        }
    }

    private void AddQuantityType(object sender, ClickedEventArgs e)
    {
        _ViewModel.SelectedQtyType = null;
        _ViewModel.Clean();
        Navigation.PushModalAsync(new AdminEditQuantityTypePage(_LangService, _ViewModel));
    }
    #endregion
}


#region Edit QuantityType
public class AdminEditQuantityTypePage : PopupPage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private AdminQuantityTypesViewModel _ViewModel => (AdminQuantityTypesViewModel)BindingContext;
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
    private readonly FloatingActionButton _Save = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended,
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
    public AdminEditQuantityTypePage(
        ILanguageService languageService,
        AdminQuantityTypesViewModel viewModel) : base(languageService)
    {
        BindingContext = viewModel;
        _LangService = languageService;

        _Description = new(viewModel.DescriptionModel);

        _Close.TapGesture(() => Navigation.PopModalAsync());

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());

        _UserInputHolder.Add(_Description);
        _ContentLayout.Add(_UserInputHolder.Row(1).Column(0).ColumnSpan(3));

        switch (viewModel.EditMode)
        {
            case EditMode.Add:
                _Title.Text = _LangService.StringForKey("Add Quantity Type");
                _Save.Text = _LangService.StringForKey("Add");

                _Description.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);

                _ContentLayout.Add(_Save.Row(2).Column(0).ColumnSpan(3));
                break;
            case EditMode.Edit:
                _Title.Text = _LangService.StringForKey("Edit Quantity Type");
                _Save.Text = _LangService.StringForKey("SaveChanges");

                viewModel.DescriptionModel.Text = viewModel.SelectedQtyType.Description;

                _ContentLayout.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Auto, Star),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _Delete.Column(0),
                        _Save.Column(1)
                    }
                }.Row(2).Column(0).ColumnSpan(3));
                break;
        }

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;
        _Save.Clicked += Save;
        _Delete.Clicked += Delete;
    }
    ~AdminEditQuantityTypePage()
    {
        _Save.Clicked -= Save;
        _Delete.Clicked -= Delete;
    }
    #endregion

    #region Helpers
    private async void Delete(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LangService.StringForKey("Deleting"))
            return;

        bool delete = await DisplayAlert(
            _LangService.StringForKey("Delete Quantity Type"),
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
        await Navigation.PopModalAsync();
    }

    private void GenericErrorOccured()
    {
        DisplayAlert(
            _LangService.StringForKey("Error"),
            _LangService.StringForKey("ErrorOccurred"),
            _LangService.StringForKey("OK"));
    }
    #endregion
}
#endregion