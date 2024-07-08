using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;

namespace Inventory.MobileApp.Controls;

public class ProfileCardView : Border 
{
    public event EventHandler? EditEmail;
    public event EventHandler? EditPhoneNumber;

    public static readonly BindableProperty UsernameProperty = BindableProperty.Create(nameof(Username), typeof(string), typeof(ProfileCardView), null);
    public string Username { get => (string)GetValue(UsernameProperty); set => SetValue(UsernameProperty, value); }

    public static readonly BindableProperty EmailProperty = BindableProperty.Create(nameof(Email), typeof(string), typeof(ProfileCardView), null);
    public string Email { get => (string)GetValue(EmailProperty); set => SetValue(EmailProperty, value); }

    public static readonly BindableProperty PhoneNumberProperty = BindableProperty.Create(nameof(PhoneNumber), typeof(string), typeof(ProfileCardView), null);
    public string PhoneNumber { get => (string)GetValue(PhoneNumberProperty); set => SetValue(PhoneNumberProperty, value); }

    private readonly Label _Header = new Label()
        .Text(LanguageService.Instance["Profile"])
        .FontSize(24)
        .Bold();
    private readonly IconLabel _Username = new IconLabel()
    {
        Header = LanguageService.Instance["Username"],
    };
    private readonly IconLabel _Email = new IconLabel()
    {
        Header = LanguageService.Instance["Email"],
        Icon = MaterialIcon.Edit
    };
    private readonly IconLabel _PhoneNumber = new IconLabel()
    {
        Header = LanguageService.Instance["Phone Number"],
        Icon = MaterialIcon.Edit
    };
    private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout
    {
        Spacing = 8
    };

    public ProfileCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _ContentLayout.Add(_Header);
        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Email);
        _ContentLayout.Add(_PhoneNumber);

        Content = _ContentLayout;

        _Email.TapGesture(() => EditEmail?.Invoke(this, EventArgs.Empty));
        _PhoneNumber.TapGesture(() => EditPhoneNumber?.Invoke(this, EventArgs.Empty));
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == UsernameProperty.PropertyName)
        {
            _Username.Text = Username;
        }
        else if (propertyName == EmailProperty.PropertyName)
        {
            _Email.Text = Email;
        }
        else if (propertyName == PhoneNumberProperty.PropertyName)
        {
            _PhoneNumber.Text = string.IsNullOrEmpty(PhoneNumber) ? "." : PhoneNumber;
        }
    }
}
