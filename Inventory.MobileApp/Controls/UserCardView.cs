using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class UserCardView : Border
{
    public static readonly BindableProperty CompanyIdProperty = BindableProperty.Create(nameof(CompanyId), typeof(int), typeof(UserCardView), -1);
    public int CompanyId { get => (int)GetValue(CompanyIdProperty); set => SetValue(CompanyIdProperty, value); }

    public static readonly BindableProperty PermissionIdProperty = BindableProperty.Create(nameof(PermissionId), typeof(int), typeof(UserCardView), -1);
    public int PermissionId { get => (int)GetValue(PermissionIdProperty); set => SetValue(PermissionIdProperty, value); }

    public static readonly BindableProperty UserIdProperty = BindableProperty.Create(nameof(UserId), typeof(int), typeof(UserCardView), -1);
    public int UserId { get => (int)GetValue(UserIdProperty); set => SetValue(UserIdProperty, value); }

    public static readonly BindableProperty UserNameProperty = BindableProperty.Create(nameof(UserName), typeof(string), typeof(UserCardView), null);
    public string UserName { get => (string)GetValue(UserNameProperty); set => SetValue(UserNameProperty, value); }

    public static readonly BindableProperty EmailProperty = BindableProperty.Create(nameof(Email), typeof(string), typeof(UserCardView), null);
    public string Email { get => (string)GetValue(EmailProperty); set=> SetValue(EmailProperty, value); }

    public static readonly BindableProperty InventoryPermissionsProperty = BindableProperty.Create(nameof(InventoryPermissions), typeof(int), typeof(UserCardView), -1);
    public int InventoryPermissions { get => (int)GetValue(InventoryPermissionsProperty); set => SetValue(InventoryPermissionsProperty, value); }

    public static readonly BindableProperty IsCompanyOwnerProperty = BindableProperty.Create(nameof(IsCompanyOwner), typeof(bool), typeof(UserCardView), false);
    public bool IsCompanyOwner { get => (bool)GetValue(IsCompanyOwnerProperty); set => SetValue(IsCompanyOwnerProperty, value); }

    private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout()
        .Spacing(8);
    private readonly Label _Header = new Label()
        .FontSize(24)
        .Bold();
    private readonly Label _Email = new Label()
        .FontSize(16);
    private readonly Label _InventoryPermLabel = new Label()
        .Text(LanguageService.Instance["Inventory Permissions"])
        .Padding(new Thickness(0, 24, 0, 0))
        .FontSize(24)
        .Bold();
    private readonly IconLabel _EditDescPerm = new();
    private readonly IconLabel _EditQtyPerm = new();
    private readonly IconLabel _EditQtyType = new();
    private readonly IconLabel _EditStatus = new();
    private readonly IconLabel _EditLocation = new();
    private readonly IconLabel _AddInventory = new();
    private readonly IconLabel _AddStatus = new();
    private readonly IconLabel _AddLocation = new();
    private readonly IconLabel _AddQtyType = new();
    private readonly IconLabel _DeleteInv = new();

    public UserCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _ContentLayout.Add(_Header);
        _ContentLayout.Add(_Email);
        _ContentLayout.Add(_InventoryPermLabel);
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _EditDescPerm.Column(0),
                _EditQtyPerm.Column(1)
            }
        });
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _AddQtyType.Column(0),
                _EditQtyType.Column(1)
            }
        });
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _AddStatus.Column(0),
                _EditStatus.Column(1)
            }
        });
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _AddInventory.Column(0),
                _DeleteInv.Column(1)
            }
        });
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _AddLocation.Column(0),
                _EditLocation.Column(1)
            }
        });

        CheckPermissions();

        Content = _ContentLayout;

        _EditDescPerm.Toggled += EditedDescriptionPermission;
        _EditQtyPerm.Toggled += EditedQuantutyPermission;
        _EditQtyType.Toggled += EditedQuantityTypePermission;
        _EditStatus.Toggled += EditedStatusPermission;
        _EditLocation.Toggled += EditedLocationPermission;
        _AddInventory.Toggled += EditedAddInvPermission;
        _AddStatus.Toggled += EditedAddStatusPermission;
        _AddLocation.Toggled += EditedAddLocationPermission;
        _AddQtyType.Toggled += EditedAddQuantityTypePermission;
        _DeleteInv.Toggled += EditedDeleteInvPermission;
    }

    private void EditedDeleteInvPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedAddQuantityTypePermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedAddLocationPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedAddStatusPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedAddInvPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedLocationPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedStatusPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedQuantityTypePermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedQuantutyPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();
    private void EditedDescriptionPermission(object? sender, ToggledEventArgs e) => PermissionsHaveBeenEdited();

    private async void PermissionsHaveBeenEdited()
    {
        int editDesc = 0;
        int editQty = 0;
        int editQtyType = 0;
        int editLocation = 0;
        int editStatus = 0;
        int deleteInv = 0;
        int addInv = 0;
        int addQtyType = 0;
        int addStatus = 0;
        int addLocation = 0;
        
        if (_EditDescPerm.IsToggled)
            editDesc = (int)Models.InventoryPermissions.CanEditDesc;
        if (_EditQtyPerm.IsToggled)
            editQty = (int)Models.InventoryPermissions.CanEditQty;
        if (_EditQtyType.IsToggled)
            editQtyType = (int)Models.InventoryPermissions.CanEditQtyType;
        if (_EditLocation.IsToggled)
            editLocation = (int)Models.InventoryPermissions.CanEditLocation;
        if (_EditStatus.IsToggled)
            editStatus = (int)Models.InventoryPermissions.CanEditStatus;
        if (_DeleteInv.IsToggled)
            deleteInv = (int)Models.InventoryPermissions.CanDeleteInv;
        if (_AddInventory.IsToggled)
            addInv = (int)Models.InventoryPermissions.CanAddInventory;
        if (_AddQtyType.IsToggled)
            addQtyType = (int)Models.InventoryPermissions.CanAddQtyType;
        if (_AddStatus.IsToggled)
            addStatus = (int)Models.InventoryPermissions.CanAddStatus;
        if (_AddLocation.IsToggled)
            addLocation = (int)Models.InventoryPermissions.CanAddLocation;

        int permissions = editDesc + editQty + editQtyType + editLocation + editStatus + deleteInv + addInv + addQtyType + addStatus + addLocation;

        var response = await NetworkService.Post<bool>(Endpoints.updatePermission, new UserPermissions
        {
            Id = PermissionId,
            CompanyId = CompanyId,
            UserId = UserId,
            InventoryPermissions = permissions
        });
    }

    private void CheckPermissions()
    {
        bool canEditDesc = PermsUtils.IsAllowed(Models.InventoryPermissions.CanEditDesc, InventoryPermissions);
        bool canEditQty = PermsUtils.IsAllowed(Models.InventoryPermissions.CanEditQty, InventoryPermissions);
        bool canEditQtyType = PermsUtils.IsAllowed(Models.InventoryPermissions.CanEditQtyType, InventoryPermissions);
        bool canEditStatus = PermsUtils.IsAllowed(Models.InventoryPermissions.CanEditStatus, InventoryPermissions);
        bool canEditLocation = PermsUtils.IsAllowed(Models.InventoryPermissions.CanEditLocation, InventoryPermissions);
        bool canAddInventory = PermsUtils.IsAllowed(Models.InventoryPermissions.CanAddInventory, InventoryPermissions);
        bool canAddStatus = PermsUtils.IsAllowed(Models.InventoryPermissions.CanAddStatus, InventoryPermissions);
        bool canAddLocation = PermsUtils.IsAllowed(Models.InventoryPermissions.CanAddLocation, InventoryPermissions);
        bool canAddQtyType = PermsUtils.IsAllowed(Models.InventoryPermissions.CanAddQtyType, InventoryPermissions);
        bool canDeleteInv = PermsUtils.IsAllowed(Models.InventoryPermissions.CanDeleteInv, InventoryPermissions);

        var user = SessionService.CurrentUser;
        if (user.IsCompanyOwner)
        {
            _EditDescPerm.Header = LanguageService.Instance["Edit Description"];
            _EditDescPerm.Icon = "";
            _EditDescPerm.LabelType = IconLabelType.Toggle;
            _EditDescPerm.IsToggled = canEditDesc;

            _EditQtyPerm.Header = LanguageService.Instance["Edit Quantity"];
            _EditQtyPerm.Icon = "";
            _EditQtyPerm.LabelType = IconLabelType.Toggle;
            _EditQtyPerm.IsToggled = canEditQty;

            _EditQtyType.Header = LanguageService.Instance["Edit Quantity Type"];
            _EditQtyType.Icon = "";
            _EditQtyType.LabelType = IconLabelType.Toggle;
            _EditQtyType.IsToggled = canEditQtyType;

            _EditStatus.Header = LanguageService.Instance["Edit Status"];
            _EditStatus.Icon = "";
            _EditStatus.LabelType = IconLabelType.Toggle;
            _EditStatus.IsToggled = canEditStatus;

            _EditLocation.Header = LanguageService.Instance["Edit Location"];
            _EditLocation.Icon = "";
            _EditLocation.LabelType = IconLabelType.Toggle;
            _EditLocation.IsToggled = canEditLocation;

            _AddInventory.Header = LanguageService.Instance["Add Inventory"];
            _AddInventory.Icon = "";
            _AddInventory.LabelType = IconLabelType.Toggle;
            _AddInventory.IsToggled = canAddInventory;

            _AddLocation.Header = LanguageService.Instance["Add Location"];
            _AddLocation.Icon = "";
            _AddLocation.LabelType = IconLabelType.Toggle;
            _AddLocation.IsToggled = canAddLocation;

            _AddQtyType.Header = LanguageService.Instance["Add Quantity Type"];
            _AddQtyType.Icon = "";
            _AddQtyType.LabelType = IconLabelType.Toggle;
            _AddQtyType.IsToggled = canAddQtyType;

            _AddStatus.Header = LanguageService.Instance["Add Status"];
            _AddStatus.Icon = "";
            _AddStatus.LabelType = IconLabelType.Toggle;
            _AddStatus.IsToggled = canAddStatus;

            _DeleteInv.Header = LanguageService.Instance["Delete Inventory"];
            _DeleteInv.Icon = "";
            _DeleteInv.LabelType = IconLabelType.Toggle;
            _DeleteInv.IsToggled = canDeleteInv;
        }
        else
        {
            _EditDescPerm.Text = LanguageService.Instance["Edit Description"];
            _EditDescPerm.Icon = canEditDesc ? MaterialIcon.Check : MaterialIcon.Close;
            _EditDescPerm.LabelType = IconLabelType.Text;

            _EditQtyPerm.Text = LanguageService.Instance["Edit Quantity"];
            _EditQtyPerm.Icon = canEditQty ? MaterialIcon.Check : MaterialIcon.Close;
            _EditQtyPerm.LabelType = IconLabelType.Text;

            _EditQtyType.Text = LanguageService.Instance["Edit Quantity Type"];
            _EditQtyType.Icon = canEditQtyType ? MaterialIcon.Check : MaterialIcon.Close;
            _EditQtyType.LabelType = IconLabelType.Text;

            _EditStatus.Text = LanguageService.Instance["Edit Status"];
            _EditStatus.Icon = canEditStatus ? MaterialIcon.Check : MaterialIcon.Close;
            _EditStatus.LabelType = IconLabelType.Text;

            _EditLocation.Text = LanguageService.Instance["Edit Location"];
            _EditLocation.Icon = canEditLocation ? MaterialIcon.Check : MaterialIcon.Close;
            _EditLocation.LabelType = IconLabelType.Text;

            _AddInventory.Text = LanguageService.Instance["Add Inventory"];
            _AddInventory.Icon = canAddInventory ? MaterialIcon.Check : MaterialIcon.Close;
            _AddInventory.LabelType = IconLabelType.Text;

            _AddLocation.Text = LanguageService.Instance["Add Location"];
            _AddLocation.Icon = canAddLocation ? MaterialIcon.Check : MaterialIcon.Close;
            _AddLocation.LabelType = IconLabelType.Text;

            _AddQtyType.Text = LanguageService.Instance["Add Quantity Type"];
            _AddQtyType.Icon = canAddQtyType ? MaterialIcon.Check : MaterialIcon.Close;
            _AddQtyType.LabelType = IconLabelType.Text;

            _AddStatus.Text = LanguageService.Instance["Add Status"];
            _AddStatus.Icon = canAddStatus ? MaterialIcon.Check : MaterialIcon.Close;
            _AddStatus.LabelType = IconLabelType.Text;

            _DeleteInv.Text = LanguageService.Instance["Delete Inventory"];
            _DeleteInv.Icon = canDeleteInv ? MaterialIcon.Check : MaterialIcon.Close;
            _DeleteInv.LabelType = IconLabelType.Text;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == UserNameProperty.PropertyName || propertyName == IsCompanyOwnerProperty.PropertyName)
        {
            _Header.Text = $"{UserName} {(IsCompanyOwner ? $"({LanguageService.Instance["Owner"]})" : "")}";
        }
        else if (propertyName == EmailProperty.PropertyName)
        {
            _Email.Text = Email;
        }
        else if (propertyName == InventoryPermissionsProperty.PropertyName)
        {
            CheckPermissions();
        }
    }
}
