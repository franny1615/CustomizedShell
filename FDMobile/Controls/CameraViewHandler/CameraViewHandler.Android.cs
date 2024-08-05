using Android.Widget;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
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

                        cameraProvider?.UnbindAll();
                        cameraProvider?.BindToLifecycle(
                            ((MauiAppCompatActivity)Platform.CurrentActivity!), 
                            cameraSelector, 
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