using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FloatingActionButton = Maui.Components.Controls.FloatingActionButton;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminUsersPage : BasePage
{
    #region Private Properties
    private AdminUsersViewModel _ViewModel => (AdminUsersViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly MaterialList<User> _Search;
    #endregion

    #region Constructor
    public AdminUsersPage(
        ILanguageService languageService,
        AdminUsersViewModel adminUsersVM) : base(languageService)
    {
        BindingContext = adminUsersVM;
        _LangService = languageService;

        Title = _LangService.StringForKey("Employees");

        _Search = new(_LangService.StringForKey("NoUsers"), MaterialIcon.Person, new DataTemplate(() =>
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
        }), adminUsersVM, isEditable: true);

        Content = _Search;
        _Search.AddItemClicked += AddUser;
    }
    ~AdminUsersPage()
    {
        _Search.AddItemClicked -= AddUser;
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
    private void UserClicked(object sender, EventArgs e)
    {
        if (sender is MaterialCardView card && 
            card.BindingContext is User user)
        {
            _ViewModel.SelectedUser = user;
            Navigation.PushModalAsync(new AdminEditUserPopupPage(_LangService, _ViewModel.EditUsersViewModel));
        }
    }

    private void AddUser(object sender, ClickedEventArgs e)
    {
        _ViewModel.SelectedUser = null;
        Navigation.PushModalAsync(new AdminEditUserPopupPage(_LangService, _ViewModel.EditUsersViewModel));
    }
    #endregion
}

public class AdminEditUserPopupPage : PopupPage
{
    #region Private Properties
    private AdminEditUsersViewModel _ViewModel => (AdminEditUsersViewModel) BindingContext;
    private readonly ILanguageService _LanguageService;
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
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Password;
    private readonly FloatingActionButton _Save = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended,
    };
    private readonly FloatingActionButton _DeleteUser = new()
    {
        FABBackgroundColor = Colors.Red,
        TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Delete, Colors.White),
        FABStyle = FloatingActionButtonStyle.Regular,
    };
    private bool _IsDeleting = false;
    #endregion

    #region Constructor
    public AdminEditUserPopupPage(
        ILanguageService languageService,
        AdminEditUsersViewModel viewModel) : base(languageService)
    {
        _LanguageService = languageService;
        BindingContext = viewModel;

        _Username = new(_ViewModel.Username);
        _Password = new(_ViewModel.Password);

        _Close.TapGesture(() => Navigation.PopModalAsync());

        switch (_ViewModel.EditMode)
        {
            case EditMode.Add:
                _Title.Text = _LanguageService.StringForKey("NewEmployee");
                _Save.Text = _LanguageService.StringForKey("AddEmployee");
                _Password.ShowStatus(null, null, Colors.DarkGray);

                _ContentLayout.Children.Add(_Save.Row(2).Column(0).ColumnSpan(3));
                break;
            case EditMode.Edit:
                _Password.ShowStatus(_LanguageService.StringForKey("EditUserPasswordInfo"), MaterialIcon.Info, Colors.Red);
                _Title.Text = _LanguageService.StringForKey("EditEmployee");
                _Save.Text = _LanguageService.StringForKey("SaveChanges");

                _ContentLayout.Children.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Auto, Star),
                    ColumnSpacing = 8,
                    Children =
                    {
                        _DeleteUser.Column(0),
                        _Save.Column(1)
                    }
                }.Row(2).Column(0).ColumnSpan(3));
                break;
        }

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());
        _ContentLayout.Children.Add(new VerticalStackLayout
        {
            Padding = 0,
            Margin = 0,
            Spacing = 8,
            Children =
            {
                _Username,
                _Password,
            }
        }.Row(1).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Save.Clicked += Save;
        _DeleteUser.Clicked += Delete;
    }

    protected override void OnDisappearing()
    {
        _Save.Clicked -= Save;
        _DeleteUser.Clicked -= Delete;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Save(object sender, ClickedEventArgs e)
    {
        if (_Save.Text == _LanguageService.StringForKey("Saving") ||
            _Save.Text == _LanguageService.StringForKey("Adding"))
        {
            return;
        }

        bool hasUsername = !string.IsNullOrEmpty(_ViewModel.Username.Text);
        bool passwordOk = _ViewModel.EditMode == EditMode.Edit ? true : !string.IsNullOrEmpty(_ViewModel.Password.Text);

        if (!hasUsername)
        {
            _Username.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }

        if (!passwordOk)
        {
            _Password.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }

        if (!hasUsername || !passwordOk)
        {
            return;
        }

        switch (_ViewModel.EditMode)
        {
            case EditMode.Edit:
                _Save.Text = _LanguageService.StringForKey("Saving");
                bool saved = await _ViewModel.EditUser();
                if (saved)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    _Save.Text = _LanguageService.StringForKey("Save");
                    await DisplayAlert(
                        _LanguageService.StringForKey("Error"), 
                        _LanguageService.StringForKey("ErrorOccurred"), 
                        _LanguageService.StringForKey("OK"));
                }
                break;
            case EditMode.Add:
                _Save.Text = _LanguageService.StringForKey("Adding");
                RegistrationResponse response = await _ViewModel.RegisterUser();
                switch (response)
                {
                    case RegistrationResponse.AlreadyExists:
                        _Username.ShowStatus(_LanguageService.StringForKey("UsernameInUse"), MaterialIcon.Info, Colors.Red);
                        _Save.Text = _LanguageService.StringForKey("AddEmployee");
                        break;
                    case RegistrationResponse.ServerError:
                        _Save.Text = _LanguageService.StringForKey("AddEmployee");
                        await DisplayAlert(
                            _LanguageService.StringForKey("Error"),
                            _LanguageService.StringForKey("ErrorOccurred"),
                            _LanguageService.StringForKey("OK"));
                        break;
                    case RegistrationResponse.SuccessfullyRegistered:
                        await Navigation.PopModalAsync();
                        break;
                }
                break;
        }
    }

    private async void Delete(object sender, ClickedEventArgs e)
    {
        if (_IsDeleting)
        {
            return;
        }

        _IsDeleting = true;

        bool delete = await DisplayAlert(
            _LanguageService.StringForKey("DeleteUser"),
            _LanguageService.StringForKey("DeleteUserPrompt"),
            _LanguageService.StringForKey("Yes"),
            _LanguageService.StringForKey("No"));

        if (delete)
        {
            bool wasDeleted = await _ViewModel.DeleteUser();
            if (wasDeleted)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert(
                    _LanguageService.StringForKey("Error"),
                    _LanguageService.StringForKey("ErrorOccurred"),
                    _LanguageService.StringForKey("OK"));
            }
        }

        _IsDeleting = false;
    }
    #endregion
}