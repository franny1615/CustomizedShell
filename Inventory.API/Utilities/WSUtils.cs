using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Inventory.API.Utilities;

public static class WSUtils
{
    public static async Task CheckServerStatus(HttpContext context)
    {
        using (var ws = await context.WebSockets.AcceptWebSocketAsync())
        {
            while (ws.State == WebSocketState.Open)
            {
                string message = $"Server is live, server time is : {DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss")}";
                var bytes = Encoding.UTF8.GetBytes(message);
                await ws.SendAsync(
                    new ArraySegment<byte>(bytes, 0, bytes.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                Thread.Sleep(1000); // 1 sec.
            }
            await ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "--closed--",
                CancellationToken.None);
        }
    }

    public static async Task MoveNETV4(HttpContext context)
    {
        using (var ws = await context.WebSockets.AcceptWebSocketAsync())
        {
            var buffer = new byte[1024 * 10000]; // 10mb potential packet size

            while (ws.State == WebSocketState.Open)
            {
                try
                {
                    var receiveResult = await ws.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    string content = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    MoveNetPacket? packet = JsonSerializer.Deserialize<MoveNetPacket>(content);

                    if (packet != null)
                    {
                        switch (packet.Type)
                        {
                            case MoveNetPacketType.KeepAlive:
                                var keepAlive = new MoveNetPacket { Type = MoveNetPacketType.KeepAlive };
                                string keepAliveStr = JsonSerializer.Serialize(keepAlive);
                                byte[] keepAliveBytes = Encoding.UTF8.GetBytes(keepAliveStr);
                                await ws.SendAsync(
                                    new ArraySegment<byte>(keepAliveBytes, 0, keepAliveBytes.Length),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                                break;
                            case MoveNetPacketType.Picture:
                                var img = Convert.FromBase64String(packet.Message); // Message should be an image
                                var detectionArray = MoveNetUtility.Predict(new MemoryStream(img));
                                var detection = new MoveNetPacket
                                {
                                    Type = MoveNetPacketType.DetectionResult,
                                    Detections = detectionArray.Data ?? []
                                };
                                string detectionStr = JsonSerializer.Serialize(detection);
                                byte[] detectionBytes = Encoding.UTF8.GetBytes(detectionStr);
                                await ws.SendAsync(
                                    new ArraySegment<byte>(detectionBytes, 0, detectionBytes.Length),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                                break;
                            case MoveNetPacketType.DetectionResult:
                            default:
                                // this is for mobile app side, do nothing
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(ex);
#endif
                }
            }

            await ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "connection ended",
                CancellationToken.None);
        }
    }
}
