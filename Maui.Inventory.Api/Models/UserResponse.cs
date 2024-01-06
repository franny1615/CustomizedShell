namespace Maui.Inventory.Api.Models;

public enum UserResponse
{
    NoUsername = 0,
    NoPassword = 1,
    NoEmail = 2,
    AlreadyExists = 3,
    ServerError = 4,
    SuccessfullyRegistered = 5,
    LicenseExpired = 6,
    AdminDoesNotExist = 7,
}
