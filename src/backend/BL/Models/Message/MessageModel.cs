using DAL.Entities;

namespace BL.Models.Message
{
    public class MessageModel
    {
        public int MessageId { get; set; }
        public string Sender { get; set; } = null!;
        public string Receiver { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; } 
    }
}
