using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessageService _messageService;
        public ChatHub(MessageService messageService)
        {
            _messageService = messageService;
        }

        private string RoomIdAsString(long roomId) => $"room-{roomId}";

        public async Task JoinRoom(long roomId) => await Groups.AddToGroupAsync(Context.ConnectionId, RoomIdAsString(roomId));

        public async Task LeaveRoom(long roomId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomIdAsString(roomId));

        public async Task SendMessage(string fromId, long roomId, string message)
        {
            await _messageService.SendMessage(fromId, roomId, message);
            await Clients.Group(RoomIdAsString(roomId)).SendAsync("ReceiveMessage", fromId, message);
        }
    }
}
