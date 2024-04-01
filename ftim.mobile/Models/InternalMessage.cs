using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ftim.mobile;

public class InternalMessage : ValueChangedMessage<object>
{
    public InternalMessage(object message) : base(message) { }
}
