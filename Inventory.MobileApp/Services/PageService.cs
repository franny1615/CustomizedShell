using Inventory.MobileApp.Pages;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Services;

public static class PageService
{
    public static LandingPage Landing()
    {
        return new LandingPage();
    }

    public static RegisterPage Register()
    {
        return new RegisterPage(new RegisterViewModel());
    }
}
