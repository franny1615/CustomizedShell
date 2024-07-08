using Inventory.MobileApp.Models;
using Inventory.MobileApp.Pages;
using Inventory.MobileApp.Pages.Components;
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

    public static UserInputPopupPage TakeUserInput(
        string title,
        string existingText,
        Keyboard keyboard,
        Action<string> submitted,
        Action canceled)
    {
        return new UserInputPopupPage(title, existingText, keyboard, submitted, canceled);
    }

    public static PickLocationPopupPage PickLocation(
        string title,
        Action<Models.Location> picked,
        Action canceled)
    {
        return new PickLocationPopupPage(new LocationSearchViewModel(), title, picked, canceled);
    }

    public static PickQuantityTypePopupPage PickQtyType(
        string title,
        Action<QuantityType> picked,
        Action canceled)
    {
        return new PickQuantityTypePopupPage(new QuantityTypesSearchViewModel(), title, picked, canceled);
    }

    public static PickStatusPopupPage PickStatus(
        string title,
        Action<Status> picked,
        Action canceled)
    {
        return new PickStatusPopupPage(new StatusSearchViewModel(), title, picked, canceled);
    }

    public static AddInventoryPage AddInventory(
        string title,
        Models.Inventory baseInventory,
        Action<Models.Inventory> add)
    {
        return new AddInventoryPage(title, baseInventory, add); 
    }

    public static ProfilePage Profile()
    {
        return new ProfilePage();
    }
}
