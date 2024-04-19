using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Microsoft.Maui.Platform;

namespace Inventory.MobileApp.Pages;

public class BasePage: ContentPage
{
    public event EventHandler? LanguageChanged;

    public BasePage()
    {
        HideSoftInputOnTapped = true;
        #if IOS
        KeyboardAutoManagerScroll.Disconnect();
        #endif
        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => { DealWithInternalMsg(msg.Value); });
        });
    }
    ~BasePage()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMsg>(this);
    }
    private void DealWithInternalMsg(InternalMessage message)
    {
        switch(message)
        {
            case InternalMessage.LanguageChanged:
                LanguageChanged?.Invoke(this, new EventArgs());
                break;
        }
    }
}
