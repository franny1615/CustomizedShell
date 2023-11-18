using Maui.Components.Controls;
#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

namespace Maui.Components;

public static class AppBuilderHostExtensions
{
    public static MauiAppBuilder UseMauiComponents(this MauiAppBuilder builder)
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(StyledEntry), (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif IOS
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;                 
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        return builder;
    }
}
