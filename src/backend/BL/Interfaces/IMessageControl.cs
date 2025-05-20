using BL.Models.Message;

namespace BL.Interfaces
{
    public interface IMessageControl
    {
        Task<List<MessageModel>> GetMessages(string email, string receiver);
    }
}
