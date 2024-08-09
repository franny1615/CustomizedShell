using Foundation;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
#if IOS
using UIKit; 
#endif 

namespace FDMobile.Pages;

public partial class MainPage : ContentPage
{
    private bool _IsConnected = false;
    private Border _Nose = new Border { BackgroundColor = Colors.Red, StrokeShape = new RoundRectangle { CornerRadius = 5 }, HeightRequest = 10, WidthRequest = 10 };
    private Border _LeftEye = new Border { BackgroundColor = Colors.Red, StrokeShape = new RoundRectangle { CornerRadius = 5 }, HeightRequest = 10, WidthRequest = 10 };
    private Border _RightEye = new Border { BackgroundColor = Colors.Red, StrokeShape = new RoundRectangle { CornerRadius = 5 }, HeightRequest = 10, WidthRequest = 10 };

	public MainPage()
	{
		InitializeComponent();
        Container.Add(_Nose);
        Container.Add(_LeftEye);
        Container.Add(_RightEye);

        _Nose.IsVisible = false;
        _LeftEye.IsVisible = false;
        _RightEye.IsVisible = false;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = ConnectToWebSocket();
    }

    private async Task ConnectToWebSocket()
    {
        var ws = new ClientWebSocket();
        ws.Options.RemoteCertificateValidationCallback = (sender, cert, chain, ssl) => true;

        ConnectedStatus.Text = "Connecting...";
        await ws.ConnectAsync(
            new Uri("wss://192.168.1.28/api/websocket/movenetv4"),
            CancellationToken.None);
        ConnectedStatus.Text = "Connected!";

        _IsConnected = true;

        var receiveTask = Task.Run(async () =>
        {
            var buffer = new byte[1024];
            while (_IsConnected)
            {
                try
                {
                    var keepAlive = new MoveNetPacket { Type = MoveNetPacketType.KeepAlive };
                    string keepAliveStr = JsonSerializer.Serialize(keepAlive);
                    byte[] keepAliveBytes = Encoding.UTF8.GetBytes(keepAliveStr);
                    await ws.SendAsync(
                        new ArraySegment<byte>(keepAliveBytes, 0, keepAliveBytes.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);

                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await Task.Delay(1000);

                    var sample = CameraView.CurrentImageSample;
                    
                    #if IOS
                    // .Scale() is very intensive appearantly so only do it at the times of fetching sample
                    var data = NSData.FromArray(sample);
                    var image = new UIImage(data);
                    sample = image.Scale(new CoreGraphics.CGSize(192, 192)).AsPNG()?.ToArray();
                    #endif 

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(sample));
                    });

                    var picture = new MoveNetPacket
                    {
                        Type = MoveNetPacketType.Picture,
                        Message = Convert.ToBase64String(sample),
                    };
                    string pictureStr = JsonSerializer.Serialize(picture);
                    byte[] pictureBytes = Encoding.UTF8.GetBytes(pictureStr);
                    await ws.SendAsync(
                        new ArraySegment<byte>(pictureBytes, 0, pictureBytes.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);

                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    MoveNetPacket? packet = JsonSerializer.Deserialize<MoveNetPacket>(message);
                    if (packet != null && packet.Detections.Length == 51)
                    {
                        // The seventeen points (x, y, prediction)
                        // [ nose, left eye, right eye, left ear, right ear, 
                        //   left shoulder, right shoulder, left elbow, 
                        //   right elbow, left wrist, right wrist, left hip, 
                        //   right hip, left knee, right knee, left ankle, right ankle ]
                        //   x,y come in as percentage from 0..1
                        
                        System.Diagnostics.Debug.WriteLine(message);

                        MainThread.BeginInvokeOnMainThread(() => 
                        {
                            var height = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;
                            var width = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;

                            var noseX = packet.Detections[0] * height;
                            var noseY = packet.Detections[1] * width;
                            var noseConfidence = packet.Detections[2];
                            
                            var leftEyeX = packet.Detections[3] * height;
                            var leftEyeY = packet.Detections[4] * width;
                            var leftEyeConfidence = packet.Detections[5];

                            var rightEyeX = packet.Detections[6] * height; 
                            var rightEyeY = packet.Detections[7] * width;
                            var rightEyeConfidence = packet.Detections[8];

                            _Nose.Margin = new Thickness(noseX, noseY, 0, 0);
                            _LeftEye.Margin = new Thickness(leftEyeX, leftEyeY, 0, 0);
                            _RightEye.Margin = new Thickness(rightEyeX, rightEyeY, 0, 0);
                            _Nose.IsVisible = true;
                            _LeftEye.IsVisible = true;
                            _RightEye.IsVisible = true;
                        });
                    }

                    if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived)
                    {
                        _IsConnected = false;
                    }
                }
                catch (Exception ex)
                {
                    _IsConnected = false;
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        });
    }
}

public class MoveNetPacket
{
    public MoveNetPacketType Type { get; set; } = MoveNetPacketType.KeepAlive;
    public string Message { get; set; } = string.Empty;
    public float[] Detections { get; set; } = [];
}

public enum MoveNetPacketType
{
    KeepAlive,
    Picture,
    DetectionResult
}