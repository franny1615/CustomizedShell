using Microsoft.Maui.Handlers;
using UIKit;

namespace FDMobile.Controls.Handlers;

public partial class CameraViewHandler : ViewHandler<CameraView, UIView>
{
    protected override UIView CreatePlatformView()
    {
        return new UIView();
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    public static void MapCameraPosition(CameraViewHandler handler, CameraView cameraView)
    {

    }
}
