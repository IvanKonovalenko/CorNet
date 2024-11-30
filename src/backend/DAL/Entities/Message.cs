public class Message
{
    public int MessageId { get; set; }
    public string SendedMessage { get; set; }
    public User Sender { get; set; }
    public User Recipient { get; set; }
}