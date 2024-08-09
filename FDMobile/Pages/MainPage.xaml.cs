using SkiaSharp;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace FDMobile.Pages;

public partial class MainPage : ContentPage
{
    private bool _IsConnected = false;

	public MainPage()
	{
		InitializeComponent();
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
                    if (packet != null)
                    {
                        // TODO: draw the points on the screen.
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