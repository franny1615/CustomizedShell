using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Maui.Inventory.Models;

public class InternalMessage : ValueChangedMessage<string>
{
    public InternalMessage(string message) : base(message) { }
}
