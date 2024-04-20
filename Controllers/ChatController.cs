using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace webchat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {

        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [Route("/getMessages")]
        public async Task GetMessages()
        {

        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                 using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (true)
                {
                    var message = "The current time is : " + DateTime.Now.ToString("HH:mm:ss");
                    var bytes = Encoding.UTF8.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    if (ws.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        Console.WriteLine($"sending message {message}");
                        await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
