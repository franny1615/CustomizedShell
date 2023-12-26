namespace Maui.Inventory.Api.Models;

public enum UserResponse
{
    NoUsername = 0,
    NoPassword = 1,
    NoEmail = 2,
    AlreadyExists = 2,
    ServerError = 3,
    SuccessfullyRegistered = 4,
    LicenseExpired = 5,
    AdminDoesNotExist = 6,
}
