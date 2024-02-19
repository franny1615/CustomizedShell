using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminStatusesPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private AdminStatusesViewModel _ViewModel => (AdminStatusesViewModel)BindingContext;
    private readonly MaterialList<Models.Status> _Search;
    #endregion

    #region Constructor
    public AdminStatusesPage(
        ILanguageService languageService,
        AdminStatusesViewModel statusesVM) : base(languageService)
    {
        BindingContext = statusesVM;

        _LangService = languageService;
        Title = _LangService.StringForKey("Statuses");

        _Search = new(noItemsText: _LangService.StringForKey("No Statuses."), noItemsIcon: MaterialIcon.Check_circle, new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "Description");
            view.SupportingText = null;
            view.Icon = MaterialIcon.Check_circle;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += StatusClicked;

            return view;
        }), statusesVM, isEditable: true);

        Content = _Search;
        _Search.AddItemClicked += AddStatus;
    }
    ~AdminStatusesPage()
    {
        _Search.AddItemClicked -= AddStatus;
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
    private void StatusClicked(object sender, EventArgs e)
    {
        if (sender is MaterialCardView card && card.BindingContext is Status status)
        {
            _ViewModel.SelectedStatus = status;
            _ViewModel.Clean();
            Navigation.PushModalAsync(new AdminEditStatusPage(_LangService, _ViewModel));
        }
    }

    private void AddStatus(object sender, ClickedEventArgs e)
    {
        _ViewModel.SelectedStatus = null;
        _ViewModel.Clean();
        Navigation.PushModalAsync(new AdminEditStatusPage(_LangService, _ViewModel));
    }
    #endregion
}

#region Edit Status
public class AdminEditStatusPage : PopupPage
{
    #region Private Properties
    private readonly ILanguageService _LangService;  
    private AdminStatusesViewModel _ViewModel => (AdminStatusesViewModel) BindingContext;
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
    public AdminEditStatusPage(
        ILanguageService languageService,
        AdminStatusesViewModel viewModel) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = viewModel;

        _Description = new(viewModel.DescriptionModel);

        _Close.TapGesture(() => Navigation.PopModalAsync());

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());

        _UserInputHolder.Add(_Description);
        _ContentLayout.Add(_UserInputHolder.Row(1).Column(0).ColumnSpan(3));

        switch(viewModel.EditMode)
        {
            case EditMode.Add:
                _Title.Text = _LangService.StringForKey("Add Status");
                _Save.Text = _LangService.StringForKey("Add");

                _Description.ShowStatus("", "", Application.Current.Resources["Primary"] as Color);

                _ContentLayout.Add(_Save.Row(2).Column(0).ColumnSpan(3));
                break;
            case EditMode.Edit:
                _Title.Text = _LangService.StringForKey("Edit Status");
                _Save.Text = _LangService.StringForKey("SaveChanges");

                viewModel.DescriptionModel.Text = viewModel.SelectedStatus.Description;

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
    ~AdminEditStatusPage()
    {
        _Save.Clicked -= Save;
        _Delete.Clicked -= Delete;
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
    private async void Delete(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LangService.StringForKey("Deleting"))
            return;

        bool delete = await DisplayAlert(
            _LangService.StringForKey("Delete Status"),
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