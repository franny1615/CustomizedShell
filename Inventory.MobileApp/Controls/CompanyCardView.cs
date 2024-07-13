using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class CompanyCardView : Border
{
    public event EventHandler? EditName;
    public event EventHandler? EditAddress1;
    public event EventHandler? EditAddress2;
    public event EventHandler? EditAddress3;
    public event EventHandler? EditCountry;
    public event EventHandler? EditCity;
    public event EventHandler? EditState;
    public event EventHandler? EditZip;

    public static readonly BindableProperty IDProperty = BindableProperty.Create(nameof(ID), typeof(int), typeof(CompanyCardView), -1);
    public int ID { get => (int)GetValue(IDProperty); set => SetValue(IDProperty, value); }

    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(CompanyCardView), null);
    public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }

    public static readonly BindableProperty Address1Property = BindableProperty.Create(nameof(Address1), typeof(string), typeof(CompanyCardView), null);
    public string Address1 { get => (string)GetValue(Address1Property); set => SetValue(Address1Property, value); }

    public static readonly BindableProperty Address2Property = BindableProperty.Create(nameof(Address2), typeof(string), typeof(CompanyCardView), null);
    public string Address2 { get => (string)GetValue(Address2Property); set => SetValue(Address2Property, value); }

    public static readonly BindableProperty Address3Property = BindableProperty.Create(nameof(Address3), typeof(string), typeof(CompanyCardView), null);
    public string Address3 { get => (string)GetValue(Address3Property); set => SetValue(Address3Property, value); }

    public static readonly BindableProperty CountryProperty = BindableProperty.Create(nameof(Country), typeof(string), typeof(CompanyCardView), null);
    public string Country { get => (string)GetValue(CountryProperty); set => SetValue(CountryProperty, value); }

    public static readonly BindableProperty CityProperty = BindableProperty.Create(nameof(City), typeof(string), typeof(CompanyCardView), null);
    public string City { get => (string)GetValue(CityProperty); set => SetValue(CityProperty, value); }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(string), typeof(CompanyCardView), null);
    public string State { get => (string)GetValue(StateProperty); set => SetValue(StateProperty, value); }

    public static readonly BindableProperty ZipProperty = BindableProperty.Create(nameof(Zip), typeof(string), typeof(CompanyCardView), null);
    public string Zip { get => (string)GetValue(ZipProperty); set => SetValue(ZipProperty, value); }

    public static readonly BindableProperty LicenseExpiresOnProperty = BindableProperty.Create(nameof(LicenseExpiresOn), typeof(DateTime), typeof(CompanyCardView), null);
    public DateTime? LicenseExpiresOn { get => (DateTime)GetValue(LicenseExpiresOnProperty); set => SetValue(LicenseExpiresOnProperty, value); }

    private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout
    {
        Spacing = 8
    };
    private readonly Label _Header = new Label()
        .Text(LanguageService.Instance["Company"])
        .FontSize(24)
        .Bold();
    private readonly IconLabel _IDLabel = new IconLabel()
    {
        Header = LanguageService.Instance["Company Code (for employee signups)"]
    };
    private readonly IconLabel _Name = new IconLabel
    {
        Header = LanguageService.Instance["Company Name"],
    };
    private readonly IconLabel _Address1 = new IconLabel
    {
        Header = LanguageService.Instance["Address 1"],
    };
    private readonly IconLabel _Address2 = new IconLabel
    {
        Header = LanguageService.Instance["Address 2"],
    };
    private readonly IconLabel _Address3 = new IconLabel
    {
        Header = LanguageService.Instance["Address 3"],
    };
    private readonly IconLabel _Country = new IconLabel
    {
        Header = LanguageService.Instance["Country"],
    };
    private readonly IconLabel _City = new IconLabel
    {
        Header = LanguageService.Instance["City"],
    };
    private readonly IconLabel _State = new IconLabel
    {
        Header = LanguageService.Instance["State"],
    };
    private readonly IconLabel _Zip = new IconLabel
    {
        Header = LanguageService.Instance["Zip"],
    };
    private readonly IconLabel _LicenseExpiresOn = new IconLabel
    {
        Header = LanguageService.Instance["License Expires On"],
    };

    public CompanyCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _ContentLayout.Add(_Header);
        _ContentLayout.Add(_IDLabel);
        _ContentLayout.Add(_Name);
        _ContentLayout.Add(_Address1);
        _ContentLayout.Add(_Address2);
        _ContentLayout.Add(_Address3);
        _ContentLayout.Add(_Country);
        _ContentLayout.Add(_City);
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            Children =
            {
                _State.Column(0),
                _Zip.Column(1),
            }
        });
        _ContentLayout.Add(_LicenseExpiresOn);

        if (SessionService.CurrentUser.IsCompanyOwner)
        {
            _Name.Icon = MaterialIcon.Edit;
            _Address1.Icon = MaterialIcon.Edit;
            _Address2.Icon = MaterialIcon.Edit;
            _Address3.Icon = MaterialIcon.Edit;
            _Country.Icon = MaterialIcon.Edit;
            _City.Icon = MaterialIcon.Edit;
            _State.Icon = MaterialIcon.Edit;
            _Zip.Icon = MaterialIcon.Edit;

            _Name.TapGesture(() => EditName?.Invoke(this, EventArgs.Empty));
            _Address1.TapGesture(() => EditAddress1?.Invoke(this, EventArgs.Empty));
            _Address2.TapGesture(() => EditAddress2?.Invoke(this, EventArgs.Empty));
            _Address3.TapGesture(() => EditAddress3?.Invoke(this, EventArgs.Empty));
            _Country.TapGesture(() => EditCountry?.Invoke(this, EventArgs.Empty));
            _City.TapGesture(() => EditCity?.Invoke(this, EventArgs.Empty));
            _State.TapGesture(() => EditState?.Invoke(this, EventArgs.Empty));
            _Zip.TapGesture(() => EditZip?.Invoke(this, EventArgs.Empty));
        }

        Content = _ContentLayout;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == NameProperty.PropertyName)
        {
            _Name.Text = Name;
        }
        else if (propertyName == Address1Property.PropertyName)
        {
            _Address1.Text = string.IsNullOrEmpty(Address1) ? "" : Address1;
        }
        else if (propertyName == Address2Property.PropertyName)
        {
            _Address2.Text = string.IsNullOrEmpty(Address2) ? "" : Address2;
        }
        else if (propertyName == Address3Property.PropertyName)
        {
            _Address3.Text = string.IsNullOrEmpty(Address3) ? "" : Address3;
        }
        else if (propertyName == CountryProperty.PropertyName)
        {
            _Country.Text = string.IsNullOrEmpty(Country) ? "" : Country;
        }
        else if (propertyName == CityProperty.PropertyName)
        {
            _City.Text = string.IsNullOrEmpty(City) ? "" : City;
        }
        else if (propertyName == StateProperty.PropertyName)
        {
            _State.Text = string.IsNullOrEmpty(State) ? "" : State;
        }
        else if (propertyName == ZipProperty.PropertyName)
        {
            _Zip.Text = string.IsNullOrEmpty(Zip) ? "" : Zip;
        }
        else if (propertyName == LicenseExpiresOnProperty.PropertyName)
        {
            if (LicenseExpiresOn != null)
            {
                _LicenseExpiresOn.Text = (LicenseExpiresOn ?? DateTime.Now).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
        }
        else if (propertyName == IDProperty.PropertyName)
        {
            _IDLabel.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ID}"));
        }
    }
}
