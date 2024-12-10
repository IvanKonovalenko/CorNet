public interface IMessagesService
{
    Task SendMessage(MessageModel model, string emailSender);
    Task<List<Message>> GetMessages(string email);
}