using DAL.Entities;
using DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string receiverEmail, string messageText)
        {
            var senderEmail = Context.User?.Identity?.Name; 

            if (senderEmail == null)
                throw new HubException("Не удалось определить email отправителя.");

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Email == senderEmail);
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Email == receiverEmail);

            if (sender == null || receiver == null)
                throw new HubException("Отправитель или получатель не найден.");

            var message = new Message
            {
                SenderId = sender.UserId,
                ReceiverId = receiver.UserId,
                Text = messageText,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.Group(receiverEmail).SendAsync("ReceiveMessage", new
            {
                senderEmail,
                text = messageText,
                sentAt = message.SentAt
            });

            await Clients.Group(senderEmail).SendAsync("ReceiveMessage", new
            {
                senderEmail,
                text = messageText,
                sentAt = message.SentAt
            });
        }
        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, email);
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, email);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
