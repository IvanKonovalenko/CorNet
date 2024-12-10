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
        var sender=await ValidateUser(emailSender);
        var recepient=await ValidateUser(model.EmailRecipient);

        Message message=new Message();
        message.Recipient=recepient;
        message.Sender=sender;
        message.SendedMessage=model.SendedMessage;
        model.DateTime = DateTime.SpecifyKind(model.DateTime, DateTimeKind.Utc);


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
    public async Task<List<Message>> GetMessages(string email)
    {
        return await _context.Messages
        .Include(m => m.Sender) 
        .Include(m => m.Recipient) 
        .Where(m => m.Sender.Email == email || m.Recipient.Email == email)
        .ToListAsync();
    }
     
}