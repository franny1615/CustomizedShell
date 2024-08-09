using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
#if IOS
using Foundation;
using UIKit; 
#endif 

namespace FDMobile.Pages;

public partial class MainPage : ContentPage
{
    private DetectionsDrawable _Detections = new();
    private bool _IsConnected = false;

	public MainPage()
	{
		InitializeComponent();
        TheCanvas.Drawable = _Detections;
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
                    var sample = CameraView.CurrentImageSample;

                    if (sample == null || (sample != null && sample.Length == 0))
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    
                    #if IOS
                    // .Scale() is very intensive appearantly so only do it at the times of fetching sample
                    var data = NSData.FromArray(sample);
                    UIImage theImage = new UIImage(data);
                    await MainThread.InvokeOnMainThreadAsync(() => 
                    {
                        theImage = theImage.GetImageWithHorizontallyFlippedOrientation();
                    });
                    sample = theImage?
                        .Scale(new CoreGraphics.CGSize(192, 192))
                        .AsPNG()?
                        .ToArray() ?? [];
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

                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    MoveNetPacket? packet = JsonSerializer.Deserialize<MoveNetPacket>(message);
                    if (packet != null && packet.Detections.Length == 51)
                    {
                        MainThread.BeginInvokeOnMainThread(() => 
                        {
                            _Detections.Packet = packet;
                            TheCanvas.Invalidate();   
                        });
                    }

                    if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived)
                    {
                        _IsConnected = false;
                        MainThread.BeginInvokeOnMainThread(() => 
                        {
                            ConnectedStatus.Text = "Disconnected";
                        });
                    }
                }
                catch (Exception ex)
                {
                    _IsConnected = false;
                    System.Diagnostics.Debug.WriteLine(ex);

                    MainThread.BeginInvokeOnMainThread(() => 
                    {
                        ConnectedStatus.Text = "Disconnected";
                    });
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