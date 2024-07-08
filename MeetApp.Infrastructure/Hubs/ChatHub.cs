using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MeetApp.Infrastructure.Hubs
{

    public class ChatHub : Hub
    {
        private static class Users
        {
            public static IDictionary<string, string> list = new Dictionary<string, string>();

        }


        private readonly ILogger<ChatHub> _logger;
        private readonly IHttpContextAccessor _httpCtx;


        public ChatHub(
            ILogger<ChatHub> logger,
            IHttpContextAccessor httpCtx)
        {
            _logger = logger;
            _httpCtx = httpCtx;

        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"New connection established : {Context.ConnectionId}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task JoinRoom(string roomId, string userId)
        {
            Users.list.Add(Context.ConnectionId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("user-connected", userId);
        }

        public async Task SendMessage(string message)
        {



            var currentUsername = _httpCtx.HttpContext.User.Identity.Name;
            var senderEmail = _httpCtx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            await Clients.All.SendAsync("ReceiveMessage", currentUsername, senderEmail, message); ;
        }
    }

}
