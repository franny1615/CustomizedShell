using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using System.Runtime.CompilerServices;

namespace Inventory.MobileApp.Pages;

public class ProfilePage : BasePage
{
    private readonly ScrollView _ContentScroll = new ScrollView()
    {
        Orientation = ScrollOrientation.Vertical,
    };
    private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout
    {
        Spacing = 12,
        Padding = 8
    };
    private readonly ProfileCardView _ProfileCard = new ProfileCardView();
    private readonly CompanyCardView _CompanyCard = new CompanyCardView();

    public ProfilePage()
    {
        Title = LanguageService.Instance["Profile"];

        _ProfileCard.BindingContext = SessionService.CurrentUser;
        _ProfileCard.SetBinding(ProfileCardView.UsernameProperty, "UserName");
        _ProfileCard.SetBinding(ProfileCardView.EmailProperty, "Email");
        _ProfileCard.SetBinding(ProfileCardView.PhoneNumberProperty, "PhoneNumber");

        _CompanyCard.BindingContext = SessionService.CurrentCompany;
        _CompanyCard.SetBinding(CompanyCardView.NameProperty, "Name");
        _CompanyCard.SetBinding(CompanyCardView.Address1Property, "Address1");
        _CompanyCard.SetBinding(CompanyCardView.Address2Property, "Address2");
        _CompanyCard.SetBinding(CompanyCardView.Address3Property, "Address3");
        _CompanyCard.SetBinding(CompanyCardView.CountryProperty, "Country");
        _CompanyCard.SetBinding(CompanyCardView.CityProperty, "City");
        _CompanyCard.SetBinding(CompanyCardView.StateProperty, "State");
        _CompanyCard.SetBinding(CompanyCardView.ZipProperty, "Zip");
        _CompanyCard.SetBinding(CompanyCardView.LicenseExpiresOnProperty, "LicenseExpiresOn");

        _ContentLayout.Add(_ProfileCard);
        _ContentLayout.Add(_CompanyCard);
        _ContentScroll.Content = _ContentLayout;

        Content = _ContentScroll;
    }
}
