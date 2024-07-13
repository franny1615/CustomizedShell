using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

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
        _CompanyCard.SetBinding(CompanyCardView.IDProperty, "Id");
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

        _ProfileCard.EditEmail += EditEmail;
        _ProfileCard.EditPhoneNumber += EditPhoneNumber;

        _CompanyCard.EditName += EditName;
        _CompanyCard.EditAddress1 += EditAddress1;
        _CompanyCard.EditAddress2 += EditAddress2;
        _CompanyCard.EditAddress3 += EditAddress3;
        _CompanyCard.EditCountry += EditCountry;
        _CompanyCard.EditCity += EditCity;
        _CompanyCard.EditState += EditState;
        _CompanyCard.EditZip += EditZip;

        Content = _ContentScroll;
    }

    private void EditEmail(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Email"],
            SessionService.CurrentUser.Email,
            Keyboard.Plain,
            submitted: async (email) => 
            {
                if (string.IsNullOrEmpty(email))
                    return;
                
                SessionService.CurrentUser.Email = email;
                await UpdateProfile();
            },
            canceled: () => { }
        ));
    }

    private void EditPhoneNumber(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Phone Number"],
            SessionService.CurrentUser.PhoneNumber,
            Keyboard.Plain,
            submitted: async (phoneNumber) => 
            {
                if (string.IsNullOrEmpty(phoneNumber))
                    return;
                
                SessionService.CurrentUser.PhoneNumber = phoneNumber;
                await UpdateProfile();
            },
            canceled: () => { }
        ));
    }

    private void EditName(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Company Name"],
            SessionService.CurrentCompany.Name,
            Keyboard.Plain,
            submitted: async (name) => 
            {
                if (string.IsNullOrEmpty(name))
                    return;
                
                SessionService.CurrentCompany.Name = name;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditAddress1(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Address1"],
            SessionService.CurrentCompany.Address1,
            Keyboard.Plain,
            submitted: async (address1) => 
            {
                if (string.IsNullOrEmpty(address1))
                    return;
                
                SessionService.CurrentCompany.Address1 = address1;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditAddress2(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Address2"],
            SessionService.CurrentCompany.Address2,
            Keyboard.Plain,
            submitted: async (address2) => 
            {
                if (address2 == null)
                    address2 = string.Empty;
                
                SessionService.CurrentCompany.Address2 = address2;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditAddress3(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Address3"],
            SessionService.CurrentCompany.Address3,
            Keyboard.Plain,
            submitted: async (address3) => 
            {
                if (address3 == null)
                    address3 = string.Empty;

                SessionService.CurrentCompany.Address3 = address3;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditCountry(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Country"],
            SessionService.CurrentCompany.Country,
            Keyboard.Plain,
            submitted: async (country) => 
            {
                if (string.IsNullOrEmpty(country))
                    return;
                
                SessionService.CurrentCompany.Country = country;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditCity(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit City"],
            SessionService.CurrentCompany.City,
            Keyboard.Plain,
            submitted: async (city) => 
            {
                if (string.IsNullOrEmpty(city))
                    return;
                
                SessionService.CurrentCompany.City = city;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditState(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit State"],
            SessionService.CurrentCompany.State,
            Keyboard.Plain,
            submitted: async (state) => 
            {
                if (string.IsNullOrEmpty(state))
                    return;
                
                SessionService.CurrentCompany.State = state;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private void EditZip(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Zip"],
            SessionService.CurrentCompany.Zip,
            Keyboard.Numeric,
            submitted: async (zip) => 
            {
                if (string.IsNullOrEmpty(zip))
                    return;
                
                SessionService.CurrentCompany.Zip = zip;
                await UpdateCompany();
            },
            canceled: () => { }
        ));
    }

    private async Task<NetworkResponse<bool>> UpdateCompany()
    {
        return await NetworkService.Post<bool>(Endpoints.companyUpdate, SessionService.CurrentCompany);
    }

    private async Task<NetworkResponse<bool>> UpdateProfile()
    {
        return await NetworkService.Post<bool>(Endpoints.userUpdate, SessionService.CurrentUser);
    }
}
