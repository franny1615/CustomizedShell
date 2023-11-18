using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CustomizedShell.Models;

public class InternalMessage : ValueChangedMessage<string>
{
    public InternalMessage(string message) : base(message) { }
}
