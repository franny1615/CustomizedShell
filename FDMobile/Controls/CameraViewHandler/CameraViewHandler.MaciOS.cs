using AVFoundation;
using CoreFoundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace FDMobile.Controls.Handlers;

public partial class CameraViewHandler : ViewHandler<CameraView, UIView>
{
    public bool HavePermissions { get; set; }
    public AVCaptureSession? CaptureSession { get; private set; }
    public AVCaptureDevice? BackCamera { get; private set; }
    public AVCaptureDevice? FrontCamera { get; private set; }
    public AVCaptureInput? FrontInput { get; private set; }
    public AVCaptureInput? BackInput { get; private set; }
    public AVCaptureVideoPreviewLayer? PreviewLayer { get; private set; }

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

    public static async void MapCameraPosition(
        CameraViewHandler handler, 
        CameraView cameraView)
    {
        var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
        var cameraCheck = PermissionStatus.Denied;
        if (cameraStatus == PermissionStatus.Denied || cameraStatus == PermissionStatus.Unknown)
        {
            cameraCheck = await Permissions.RequestAsync<Permissions.Camera>();
        }
        
        handler.HavePermissions = 
            cameraStatus == PermissionStatus.Granted || 
            cameraCheck == PermissionStatus.Granted;

        DispatchQueue.DefaultGlobalQueue.DispatchSync(() =>
        {
            if (!handler.HavePermissions)
                return;

            handler.CaptureSession = new AVCaptureSession();
            handler.CaptureSession.BeginConfiguration();

            if (handler.CaptureSession.CanSetSessionPreset(AVCaptureSession.Preset1280x720))
            {
                handler.CaptureSession.SessionPreset = AVCaptureSession.Preset1280x720;
            }

            handler.CaptureSession.AutomaticallyConfiguresCaptureDeviceForWideColor = true;

            handler.SetupInputs(cameraView.Position);

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                handler.SetupPreviewLayer();
            });

            handler.CaptureSession.CommitConfiguration();
            handler.CaptureSession.StartRunning();
        });
    }

    private void SetupInputs(CameraPosition position)
    {
        var rear = AVCaptureDevice.GetDefaultDevice(
            AVCaptureDeviceType.BuiltInWideAngleCamera,
            AVMediaTypes.Video,
            AVCaptureDevicePosition.Back);

        var front = AVCaptureDevice.GetDefaultDevice(
            AVCaptureDeviceType.BuiltInWideAngleCamera,
            AVMediaTypes.Video,
            AVCaptureDevicePosition.Front);

        BackCamera = rear;
        FrontCamera = front;

        if (BackCamera != null)
        {
            BackInput = AVCaptureDeviceInput.FromDevice(BackCamera);
        }
        if (FrontCamera != null)
        {
            FrontInput = AVCaptureDeviceInput.FromDevice(FrontCamera);
        }

        if (CaptureSession != null)
        {
            switch (position)
            {
                case CameraPosition.Front:
                    if (FrontInput != null)
                        CaptureSession.AddInput(FrontInput);
                    break;
                case CameraPosition.Rear:
                case CameraPosition.Unset:
                    if (BackInput != null)
                        CaptureSession.AddInput(BackInput);
                    break;
            }
        }
    }

    private void SetupPreviewLayer()
    {
        if (CaptureSession == null)
            return;

        PreviewLayer = new AVCaptureVideoPreviewLayer(CaptureSession);
        PlatformView.Layer.InsertSublayer(PreviewLayer, 0);
        PreviewLayer.Frame = PlatformView.Layer.Frame;
    }
}
