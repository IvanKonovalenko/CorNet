public interface IMessagesService
{
    Task SendMessage(MessageModel model, string emailSender);
}