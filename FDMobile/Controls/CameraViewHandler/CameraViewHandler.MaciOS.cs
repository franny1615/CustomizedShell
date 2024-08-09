using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;
using CoreImage;
using CoreGraphics;
using Microsoft.Maui.Graphics.Platform;

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
    public AVCaptureVideoDataOutput? VideoDataOutput { get; private set; }
    public DispatchQueue? VideoOutputQueue { get; private set; }

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
            handler.SetupOutputs();

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
            BackCamera.ActiveVideoMaxFrameDuration = new CMTime(1, 30);
            BackInput = AVCaptureDeviceInput.FromDevice(BackCamera);
        }
        if (FrontCamera != null)
        {
            FrontCamera.ActiveVideoMaxFrameDuration = new CMTime(1, 30);
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

    private void SetupOutputs()
    {
        VideoDataOutput = new AVCaptureVideoDataOutput();
        VideoOutputQueue = new DispatchQueue("cvVideoQueue");
        VideoDataOutput.SetSampleBufferDelegate(new VideoCaptureDelegate((image) => {
            if (image != null)
            {
                var data = image.AsPNG()?.ToArray();
                VirtualView.CurrentImageSample = data ?? [];
            }
        }), VideoOutputQueue);

        if (CaptureSession != null)
        {
            CaptureSession.AddOutput(VideoDataOutput);
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

public class VideoCaptureDelegate : NSObject, IAVCaptureVideoDataOutputSampleBufferDelegate
{
    private Action<UIImage?> _GotSample;

    public VideoCaptureDelegate(Action<UIImage?> gotSample)
    {
        _GotSample = gotSample;
    }

    [Foundation.Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
    public virtual void DidOutputSampleBuffer (
        AVCaptureOutput captureOutput, 
        CMSampleBuffer sampleBuffer, 
        AVCaptureConnection connection)
    {
        _GotSample?.Invoke(ImageFromSampleBuffer(sampleBuffer));
    }

    public UIImage? ImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
    {
        var imageBuffer = sampleBuffer.GetImageBuffer();
        if (imageBuffer == null)
            return null;

        var ciImage = CIImage.FromImageBuffer(imageBuffer);
        var context = new CIContext();

        if (ciImage == null)
            return null;
        
        UIDeviceOrientation currentOrientation = UIDevice.CurrentDevice.Orientation;
        switch (currentOrientation)
        {
            case UIDeviceOrientation.Portrait:
                ciImage = ciImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Right);
                break;
            case UIDeviceOrientation.PortraitUpsideDown:
                ciImage = ciImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Left);
                break;
            case UIDeviceOrientation.LandscapeLeft:
                ciImage = ciImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Down);
                break;
            case UIDeviceOrientation.LandscapeRight:
                ciImage = ciImage.CreateByApplyingOrientation(ImageIO.CGImagePropertyOrientation.Up);
                break;
        }

        var cgImage = context.CreateCGImage(ciImage, ciImage.Extent);
        if (cgImage == null)
            return null;

        return new UIImage(cgImage);
    }
}
