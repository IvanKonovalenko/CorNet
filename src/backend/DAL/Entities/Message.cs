namespace DAL.Entities
{
    public class Message
    {
        public int MessageId { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

    }
}
