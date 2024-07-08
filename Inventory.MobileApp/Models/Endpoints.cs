namespace Inventory.MobileApp.Models;

public static class Endpoints
{
    public const string beginEmailValidation = "api/email/beginValidation";
    public const string validateEmail = "api/email/validate";
    public const string registerCompany = "api/company/register";
    public const string companyDetails = "api/company/details";
    public const string companyUpdate = "api/company/update";
    public const string companyDelete = "api/company/delete";
    public const string registerUser = "api/user/register";
    public const string checkUsername = "api/user/check";
    public const string login = "api/user/login";
    public const string userDetails = "api/user/details";
    public const string userUpdate = "api/user/update";
    public const string userDelete = "api/user/delete";
    public const string searchStatus = "api/status/search";
    public const string insertStatus = "api/status/insert";
    public const string deleteStatus = "api/status/delete";
    public const string updateStatus = "api/status/update";
    public const string searchLocation = "api/location/search";
    public const string insertLocation = "api/location/insert";
    public const string deleteLocation = "api/location/delete";
    public const string updateLocation = "api/location/update";
    public const string searchQtyType = "api/quantityType/search";
    public const string insertQtyType = "api/quantityType/insert";
    public const string deleteQtyType = "api/quantityType/delete";
    public const string updateQtyType = "api/quantityType/update";
    public const string searchInventory = "api/inventory/search";
    public const string insertInventory = "api/inventory/insert";
    public const string deleteInventory = "api/inventory/delete";
    public const string updateInventory = "api/inventory/update";
    public const string insertPermission = "api/permissions/insert";
    public const string updatePermission = "api/permissions/update";
    public const string deletePermission = "api/permissions/delete";
    public const string getPermissionByUser = "api/permissions/detailsByUser";
}
