using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminUsersPage : BasePage
{
    #region Private Properties
    private AdminUsersViewModel _ViewModel => (AdminUsersViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly Debouncer _SearchDebouncer = new(0.5);
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star, Auto),
        RowSpacing = 8,
        Padding = 16
    };
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
    private readonly MaterialImage _UserIcon = new()
    {
        Icon = MaterialIcon.Group,
        IconColor = Application.Current.Resources["TextColor"] as Color,
        IconSize = 40
    };
    private readonly Label _NoUsers = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };
    private readonly VerticalStackLayout _NoUsersUI = new()
    {
        Spacing = 8,
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center
    };
    private bool _IsLoading = false;
    private readonly ProgressBar _BusyIndicator = new() { ZIndex = 1, WidthRequest = 200 };
    private readonly MaterialEntry _Search;
    #endregion

    #region Constructor
    public AdminUsersPage(
        ILanguageService languageService,
        AdminUsersViewModel adminUsersVM) : base(languageService)
    {
        BindingContext = adminUsersVM;
        _LangService = languageService;

        _Search = new(adminUsersVM.SearchModel);

        _NoUsers.Text = _LangService.StringForKey("NoUsers");
        Title = _LangService.StringForKey("Employees");

        _NoUsersUI.Add(_UserIcon.Center());
        _NoUsersUI.Add(_NoUsers);

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

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_UsersCollection.Row(1));
        _ContentLayout.Children.Add(_AddUser.Row(1).End().Bottom());

        ToolbarItems.Add(new ToolbarItem
        {
            IconImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Refresh, Colors.White, 30),
            Command = new Command(() =>
            {
                FetchUsers();
            })
        });

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        FetchUsers();
        _AddUser.Clicked += AddUser;
        _Search.TextChanged += SearchChanged;
    }

    protected override void OnDisappearing()
    {
        _AddUser.Clicked -= AddUser;
        _Search.TextChanged -= SearchChanged;
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
            _ContentLayout.Children.Remove(_NoUsersUI);
        }
        else
        {
            if (!_ContentLayout.Children.Contains(_NoUsersUI))
            {
                _ContentLayout.Children.Add(_NoUsersUI.Row(1));
            }
        }

        EndBusyIndicator();
    }

    private void StartBusyIndicator()
    {
        _IsLoading = true;
        _ContentLayout.Children.Add(_BusyIndicator.Top().CenterHorizontal().Row(1));
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

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        _SearchDebouncer.Debounce(() =>
        {
            FetchUsers();
        });
    }
    #endregion
}