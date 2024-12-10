public interface IMessagesService
{
    Task SendMessage(MessageModel model, string emailSender);
    Task<List<MessageModel>> GetMessages(string email);
}