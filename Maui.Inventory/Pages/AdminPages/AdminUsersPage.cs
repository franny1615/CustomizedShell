using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.ViewModels.AdminVM;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminUsersPage : BasePage
{
    #region Private Properties
    private AdminUsersViewModel _ViewModel => (AdminUsersViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly Grid _ContentLayout = new();
    private readonly FloatingActionButton _AddUser = new()
    {
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Add, Colors.White),
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular,
        ZIndex = 1,
    };
    private readonly CollectionView _UsersCollection = new()
    {
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical),
        ZIndex = 0,
    };
    private readonly Label _NoUsers = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };
    private bool _IsLoading = false;
    private readonly ProgressBar _BusyIndicator = new() { ZIndex = 1, WidthRequest = 200 };
    #endregion

    #region Constructor
    public AdminUsersPage(
        ILanguageService languageService,
        AdminUsersViewModel adminUsersVM) : base(languageService)
    {
        BindingContext = adminUsersVM;
        _LangService = languageService;

        _NoUsers.Text = _LangService.StringForKey("NoUsers");
        Title = _LangService.StringForKey("Employees");

        _UsersCollection.SetBinding(CollectionView.ItemsSourceProperty, "Users");
        _UsersCollection.ItemTemplate = new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "UserName");
            view.SetBinding(MaterialCardView.SupportingTextProperty, "Id");
            view.Icon = MaterialIcon.Person;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += UserClicked;

            return view;
        });

        _ContentLayout.Children.Add(_UsersCollection);
        _ContentLayout.Children.Add(_AddUser.End().Bottom());

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        FetchUsers();
        _AddUser.Clicked += AddUser;
    }

    protected override void OnDisappearing()
    {
        _AddUser.Clicked -= AddUser;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void FetchUsers()
    {
        StartBusyIndicator();
        await _ViewModel.GetUsers();

        if (_ViewModel.Users.Count > 0)
        {
            _ContentLayout.Children.Remove(_NoUsers);
        }
        else
        {
            if (!_ContentLayout.Children.Contains(_NoUsers))
            {
                _ContentLayout.Children.Add(_NoUsers);
            }
        }

        EndBusyIndicator();
    }

    private void StartBusyIndicator()
    {
        _ContentLayout.Padding = new Thickness(16, 0, 16, 16);
        _IsLoading = true;
        _ContentLayout.Children.Add(_BusyIndicator.Top().CenterHorizontal());
        _BusyIndicator.ProgressColor = Application.Current.Resources["Secondary"] as Color;
        _BusyIndicator.Progress = 1;
        Task.Run(async () =>
        {
            while(_IsLoading)
            {
                await _BusyIndicator.FadeTo(0.25);
                await _BusyIndicator.FadeTo(1);
            }
        });
    }

    private void EndBusyIndicator()
    {
        _ContentLayout.Padding = 16;
        _ContentLayout.Children.Remove(_BusyIndicator);
        _IsLoading = false;
    }

    private void UserClicked(object sender, EventArgs e)
    {
        if (sender is MaterialCardView card && 
            card.BindingContext is User user)
        {
            // TODO: go to edit user page
        }
    }

    private void AddUser(object sender, ClickedEventArgs e)
    {
        // TODO: go to add user page
    }
    #endregion
}