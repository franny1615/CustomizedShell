using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Maui.Inventory.Models;

public class InternalMessage : ValueChangedMessage<object>
{
    public InternalMessage(object message) : base(message) { }
}

public enum AccessMessage
{
    AdminSignedIn,
    UserSignedIn,
    AdminLogout,
    UserLogout,
    AccessTokenExpired,
    LandingPage
}

public enum RegistrationResponse
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