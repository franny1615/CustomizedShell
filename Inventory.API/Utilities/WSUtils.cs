using System.Net.WebSockets;
using System.Text;

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
}
