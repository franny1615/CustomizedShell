using Android.Graphics;
using Android.Widget;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using Java.IO;
using Java.Lang;
using Java.Util.Concurrent;
using Microsoft.Maui.Handlers;
using static Android.Views.ViewGroup;

namespace FDMobile.Controls.Handlers;

public partial class CameraViewHandler : ViewHandler<CameraView, RelativeLayout>
{
    public bool HavePermissions { get; private set; } = false;
    public PreviewView PreviewView { get; private set; }
    public IExecutorService? CameraExecutor { get; private set; }

    public IExecutorService? ImageAnalysisExecutor { get; private set; }
    public ImageAnalysis? ImageAnalysis { get; private set; }

    protected override RelativeLayout CreatePlatformView()
    {
        return new RelativeLayout(Context);
    }

    protected async override void ConnectHandler(RelativeLayout platformView)
    {
        base.ConnectHandler(platformView);
        
        var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
        var cameraCheck = PermissionStatus.Denied;
        if (cameraStatus == PermissionStatus.Denied || cameraStatus == PermissionStatus.Unknown)
        {
            cameraCheck = await Permissions.RequestAsync<Permissions.Camera>();
        }
        
        HavePermissions = 
            cameraStatus == PermissionStatus.Granted || 
            cameraCheck == PermissionStatus.Granted;

        if (HavePermissions)
        {
            CameraExecutor = Executors.NewSingleThreadExecutor();
            ImageAnalysisExecutor = Executors.NewSingleThreadExecutor();
            PreviewView = new PreviewView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
            };
            platformView.AddView(PreviewView);
        }
    }

    protected override void DisconnectHandler(RelativeLayout platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    public static void MapCameraPosition(CameraViewHandler handler, CameraView cameraView)
    {   
        if (handler.HavePermissions)
        {
            var cameraProviderFuture = ProcessCameraProvider.GetInstance(handler.Context);
            cameraProviderFuture.AddListener(
                new CameraRunnable(() =>
                {
                    try
                    {
                        var cameraProvider = cameraProviderFuture.Get() as ProcessCameraProvider;
                        var preview = new Preview.Builder().Build();
                        preview.SetSurfaceProvider(handler.PreviewView.SurfaceProvider);

                        var cameraSelector = CameraSelector.DefaultBackCamera;
                        switch (cameraView.Position)
                        {
                            case CameraPosition.Front:
                                cameraSelector = CameraSelector.DefaultFrontCamera;
                                break;
                            case CameraPosition.Rear:
                                cameraSelector = CameraSelector.DefaultBackCamera;
                                break;
                            default:
                                break;
                        }

                        handler.ImageAnalysis = new ImageAnalysis
                            .Builder()
                            .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                            .Build();
                        if (handler.ImageAnalysisExecutor != null)
                        {
                            handler.ImageAnalysis.SetAnalyzer(
                                handler.ImageAnalysisExecutor,
                                new ImgAnalyzerDelegate((sample) =>
                                {
                                    cameraView.CurrentImageSample = sample;
                                }));
                        }

                        cameraProvider?.UnbindAll();
                        cameraProvider?.BindToLifecycle(
                            ((MauiAppCompatActivity)Platform.CurrentActivity!), 
                            cameraSelector, 
                            handler.ImageAnalysis,
                            preview);
                    }
                    catch (Java.Lang.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }), 
                ContextCompat.GetMainExecutor(handler.Context));
        }
    }
}

public class CameraRunnable(Action run) : Java.Lang.Object, IRunnable
{
    public void Run()
    {
        run?.Invoke();
    }
}

public class ImgAnalyzerDelegate(
    Action<byte[]> gotSample) : Java.Lang.Object, ImageAnalysis.IAnalyzer
{
    public Android.Util.Size DefaultTargetResolution => new Android.Util.Size(1280, 720);

    public void Analyze(IImageProxy p0)
    {
        var bitmap = p0.ToBitmap();

        Matrix matrix = new();
        matrix.PostRotate(p0.ImageInfo.RotationDegrees);
        Bitmap bitmapRotated = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

        Matrix mirror = new();
        mirror.PreScale(-1, 1);
        Bitmap bitmapMirrored = Bitmap.CreateBitmap(bitmapRotated, 0, 0, bitmapRotated.Width, bitmapRotated.Height, mirror, true);

        Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(bitmapMirrored, 192, 192, false);

        MemoryStream stream = new MemoryStream();
        scaledBitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);

        byte[] data = stream.ToArray();
        gotSample?.Invoke(data);    
        bitmap.Recycle();

        p0.Close(); // important in order to get more samples
    }
}