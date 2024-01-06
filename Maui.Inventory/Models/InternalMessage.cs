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
    AlreadyExists = 3,
    ServerError = 4,
    SuccessfullyRegistered = 5,
    LicenseExpired = 6,
    AdminDoesNotExist = 7,
}