#if IOS
using Microsoft.Maui.Platform;
using UIKit;
using CoreGraphics;
#endif

namespace Inventory.MobileApp.Services;

public static class KeyboardService
{
#if IOS 
    private static object? _keyboardWillShow;
    private static object? _keyboardWillHide;
#endif 

    private static Action<double>? _kbHeightChanged;

    public static void Observe(Action<double> kbHeightChanged)
    {
        _kbHeightChanged = kbHeightChanged;
#if IOS
        _keyboardWillShow = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShowing);
        _keyboardWillHide = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHiding);
#endif
    }

    public static void EndObservation()
    {
        _kbHeightChanged = null;
#if IOS
        _keyboardWillShow = null;
        _keyboardWillHide = null;
#endif
    }

#if IOS
    private static void OnKeyboardShowing(object? sender, UIKeyboardEventArgs args)
    {
        var vc = Platform.GetCurrentUIViewController();
        if (vc == null)
            return;
        UIView? control = vc!.View?.FindFirstResponder();;
        UIView? rootUiView = vc!.View;
        if (rootUiView == null)
            return;
        CGRect kbFrame = UIKeyboard.FrameEndFromNotification(args.Notification);
        double distance = control?.GetOverlapDistance(rootUiView, kbFrame) ?? 0.0;
        if (distance > 0)
        {
            _kbHeightChanged?.Invoke(distance);
        }
    }

    private static void OnKeyboardHiding(object? sender, UIKeyboardEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() => 
        {
            _kbHeightChanged?.Invoke(0);
        });
    }
#endif
}

#if IOS
public static class UIViewExtensions
{
    /// <summary>
    /// Find the first responder in the <paramref name="view"/>'s subview hierarchy
    /// </summary>
    /// <param name="view">
    /// A <see cref="UIView"/>
    /// </param>
    /// <returns>
    /// A <see cref="UIView"/> that is the first responder or null if there is no first responder
    /// </returns>
    public static UIView? FindFirstResponder(this UIView view)
    {
        if (view.IsFirstResponder)
        {
            return view;
        }
        foreach (UIView subView in view.Subviews)
        {
            var firstResponder = subView.FindFirstResponder();
            if (firstResponder != null)
                return firstResponder;
        }
        return null;
    }

    /// <summary>
    /// Returns the view Bottom (Y + Height) coordinates relative to the rootView
    /// </summary>
    /// <returns>The view relative bottom.</returns>
    /// <param name="view">View.</param>
    /// <param name="rootView">Root view.</param>
    private static double GetViewRelativeBottom(this UIView view, UIView rootView)
    {
        // https://developer.apple.com/documentation/uikit/uiview/1622424-convertpoint
        var viewRelativeCoordinates = rootView.ConvertPointFromView(new CGPoint(0, 0), view);
        var activeViewRoundedY = Math.Round(viewRelativeCoordinates.Y, 2);

        return activeViewRoundedY + view.Frame.Height;
    }

    private static double GetOverlapDistance(double relativeBottom, UIView rootView, CGRect keyboardFrame)
    {
        // var safeAreaBottom = rootView.Window.SafeAreaInsets.Bottom;
        var pageHeight = rootView.Frame.Height;
        var keyboardHeight = keyboardFrame.Height;
        return relativeBottom - (pageHeight - keyboardHeight);
    }

    /// <summary>
    /// Returns the distance between the bottom of the View and the top of the keyboard
    /// </summary>
    /// <param name="activeView">View.</param>
    /// <param name="rootView">Root view.</param>
    /// <param name="keyboardFrame">Keyboard Frame.</param>
    /// <returns>The distance, positive if the keyboard overlaps with the View, negative otherwise</returns>
    public static double GetOverlapDistance(this UIView activeView, UIView rootView, CGRect keyboardFrame)
    {
        double bottom = activeView.GetViewRelativeBottom(rootView);
        return GetOverlapDistance(bottom, rootView, keyboardFrame);
    }
}
#endif