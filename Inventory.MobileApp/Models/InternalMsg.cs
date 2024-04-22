using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Inventory.MobileApp.Models;

public enum InternalMessage
{
    LanguageChanged,
    LoggedIn,
    LoggedOut,
    CheckAuth,
    FirstInstall,
    ThemeChanged
}

public class InternalMsg : ValueChangedMessage<InternalMessage>
{
    public InternalMsg(InternalMessage value) : base(value) {}
}
