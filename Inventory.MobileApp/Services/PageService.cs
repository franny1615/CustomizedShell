﻿using Inventory.MobileApp.Pages;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Services;

public static class PageService
{
    public static RegisterPage Register()
    {
        return new RegisterPage(new RegisterViewModel());
    }

    public static LoginPage Login()
    {
        return new LoginPage(new LoginViewModel());
    }

    public static DashboardPage Dashboard()
    {
        return new DashboardPage(new DashboardViewModel());
    }

    public static StatusSearchPage StatusSearch()
    {
        return new StatusSearchPage(new StatusSearchViewModel());
    }

    public static LocationSearchPage LocationSearch()
    {
        return new LocationSearchPage(new LocationSearchViewModel());
    }

    public static QuantityTypesSearchPage QuantityTypesSearch()
    {
        return new QuantityTypesSearchPage(new QuantityTypesSearchViewModel());
    }

    public static InventorySearchPage InventorySearch()
    {
        return new InventorySearchPage(new InventorySearchViewModel());
    }
}
