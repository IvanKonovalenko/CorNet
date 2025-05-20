using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Message;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class MessageControl : BaseService, IMessageControl
    {
        public MessageControl(AppDbContext context) : base(context)
        {

        }
        public async Task<List<MessageModel>> GetMessages(string email, string receiver)
        {
            User? user = await CheckAuthorization(email);

            User? userReceiver = await CheckReceiver(receiver);

            return await _context.Messages.Where(m =>
            m.SenderId == user.UserId && m.ReceiverId == userReceiver.UserId ||
            m.SenderId == userReceiver.UserId && m.ReceiverId == user.UserId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageModel
            {
                MessageId = m.MessageId,
                SenderEmail = m.Sender.Email,
                Receiver = m.Receiver.Email,
                Text = m.Text,
                SentAt = m.SentAt,
            }).ToListAsync();
        }
    }
}
