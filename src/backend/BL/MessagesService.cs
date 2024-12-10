using Microsoft.EntityFrameworkCore;

public class MessagesService : IMessagesService
{
    private readonly AppDbContext _context;
    public MessagesService(AppDbContext context)
    {
        _context=context;
    }
    public async Task SendMessage(MessageModel model, string emailSender)
    {
        var sender=await ValidateUser(model.EmailRecipient);
        var recepient=await ValidateUser(model.EmailRecipient);

        Message message=new Message();
        message.Recipient=recepient;
        message.Sender=sender;
        message.SendedMessage=model.SendedMessage;
        message.DateTime=model.DateTime;

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
        
    }
     public async Task<User> ValidateUser(string email)
     {
        var user= await _context.Users.Where(user=>user.Email==email).FirstOrDefaultAsync();
        if(user?.UserId ==null)
            throw new UserException();
        return user;
     }
     
}