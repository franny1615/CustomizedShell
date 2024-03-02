using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
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
        }), adminUsersVM, isEditable: AccessControl.IsLicenseValid);

        Content = _Search;
        _Search.AddItemClicked += AddUser;

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => ProcessInternalMsg(msg.Value.ToString()));
        });
    }
    ~AdminUsersPage()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMessage>(this);
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

    private void ProcessInternalMsg(string message)
    {
        if (message == "language-changed")
        {
            Title = _LangService.StringForKey("Employees");
            _ViewModel.SearchModel.Placeholder = _LangService.StringForKey("Search");
        }
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
    private readonly Label _EditInventoryPermissions = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
    private readonly MaterialToggle _CanChangeStatus = new() { Icon = MaterialIcon.Check_circle };
    private readonly MaterialToggle _CanChangeDesc = new() { Icon = MaterialIcon.Description };
    private readonly MaterialToggle _CanChangeQty = new() { Icon = MaterialIcon.Looks_one };
    private readonly MaterialToggle _CanChangeQtyType = new() { Icon = MaterialIcon.Video_label };
    private readonly MaterialToggle _CanChangeLocation = new() { Icon = MaterialIcon.Shelves };
    private readonly MaterialToggle _CanDelete = new() { Icon = MaterialIcon.Delete };
    private readonly MaterialToggle _CanAdd = new() { Icon = MaterialIcon.Add };

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

        _EditInventoryPermissions.Text = _LanguageService.StringForKey("Inventory Permissions");
        _CanChangeStatus.Text = _LanguageService.StringForKey("Change Status");
        _CanChangeDesc.Text = _LanguageService.StringForKey("Change Description");
        _CanChangeQty.Text = _LanguageService.StringForKey("Change Quantity");
        _CanChangeQtyType.Text = _LanguageService.StringForKey("Change Qty Type");
        _CanChangeLocation.Text = _LanguageService.StringForKey("Change Location");
        _CanDelete.Text = _LanguageService.StringForKey("Delete");
        _CanAdd.Text = _LanguageService.StringForKey("Add");

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

                _CanChangeStatus.IsToggled = AccessControl.CanChangeStatus(_ViewModel.EditInvPerms);
                _CanChangeDesc.IsToggled = AccessControl.CanChangeDescription(_ViewModel.EditInvPerms);
                _CanChangeQty.IsToggled = AccessControl.CanChangeQuantity(_ViewModel.EditInvPerms);
                _CanChangeQtyType.IsToggled = AccessControl.CanChangeQuantityType(_ViewModel.EditInvPerms);
                _CanChangeLocation.IsToggled = AccessControl.CanChangeLocation(_ViewModel.EditInvPerms);
                _CanDelete.IsToggled = AccessControl.CanDeleteInventory(_ViewModel.EditInvPerms);
                _CanAdd.IsToggled = AccessControl.CanAddInventory(_ViewModel.EditInvPerms);

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
        _ContentLayout.Children.Add(new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 0,
                Margin = 0,
                Spacing = 8,
                Children =
                {
                    _Username,
                    _Password,
                    _EditInventoryPermissions,
                    _CanChangeStatus,
                    _CanChangeDesc,
                    _CanChangeQty,
                    _CanChangeQtyType,
                    _CanChangeLocation,
                    _CanDelete,
                    _CanAdd
                }
            }
        }.Row(1).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;
        _Save.Clicked += Save;
        _DeleteUser.Clicked += Delete;
    }
    ~AdminEditUserPopupPage()
    {
        _Save.Clicked -= Save;
        _DeleteUser.Clicked -= Delete;
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

        int permissionsResult = 0;
        if (_CanChangeStatus.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanChangeStatus;
        }
        if (_CanChangeDesc.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanChangeDescription;
        }
        if (_CanChangeQty.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanChangeQuantity;
        }
        if (_CanChangeQtyType.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanChangeQuantityType;
        }
        if (_CanChangeLocation.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanChangeLocation;
        }
        if (_CanAdd.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanAddInventory;
        }
        if (_CanDelete.IsToggled)
        {
            permissionsResult = permissionsResult | (int)EditInventoryPerms.CanDelete;
        }

        switch (_ViewModel.EditMode)
        {
            case EditMode.Edit:
                _Save.Text = _LanguageService.StringForKey("Saving");
                bool saved = await _ViewModel.EditUser(permissionsResult);
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
                RegistrationResponse response = await _ViewModel.RegisterUser(permissionsResult);
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