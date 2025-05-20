using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Message;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL
{
    public class MessageControl : IMessageControl
    {
        private readonly AppDbContext _context;
        public MessageControl(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<MessageModel>> GetMessages(string email, string receiver)
        {
            User? user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            User? userReceiver = await _context.Users.Where(u => u.Email == receiver).FirstOrDefaultAsync();
            if (userReceiver is null) throw new ReceiverException();

             return await _context.Messages.Where(m =>
            (m.SenderId == user.UserId && m.ReceiverId == userReceiver.UserId) ||
            (m.SenderId == userReceiver.UserId && m.ReceiverId == user.UserId))
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageModel
            {
                MessageId = m.MessageId,
                Sender = m.Sender.Email,
                Receiver = m.Receiver.Email,
                Text = m.Text,
                SentAt = m.SentAt,
            }).ToListAsync();
        }
    }
}
