using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class UserSearchPage : BasePage
{
    private readonly UserSearchViewModel _ViewModel;
    private readonly SearchView<User> _Search;
    private bool _IsEditing = false;

    public UserSearchPage(UserSearchViewModel viewModel)
    {
        Title = LanguageService.Instance["Employees"];

        _ViewModel = viewModel;
        _Search = new(viewModel);
        _Search.CanAddItems = false;
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new UserCardView();
            view.SetBinding(UserCardView.BindingContextProperty, ".");
            view.SetBinding(UserCardView.UserNameProperty, "UserName");
            view.SetBinding(UserCardView.EmailProperty, "Email");
            view.SetBinding(UserCardView.IsCompanyOwnerProperty, "IsCompanyOwner");
            view.SetBinding(UserCardView.InventoryPermissionsProperty, "InventoryPermissions");
            view.SetBinding(UserCardView.PermissionIdProperty, "PermissionId");
            view.SetBinding(UserCardView.UserIdProperty, "Id");
            view.SetBinding(UserCardView.CompanyIdProperty, "CompanyID");

            return view;
        });
        _Search.AddItem += AddUser;

        Content = _Search;
    }

    private void AddUser(object? sender, EventArgs e)
    {
        // TODO: implement it some time in the future
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_IsEditing)
        {
            _Search.TriggerRefresh();
        }
    }
}
