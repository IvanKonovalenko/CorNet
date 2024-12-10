using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessagesService _messagesService;
    public MessageController(IMessagesService messagesService)
    {
        _messagesService= messagesService;
    }
    [HttpPost("SendMessage")]
    public async Task<IActionResult> SendMessage(MessageModel model)
    {
        if(ModelState.IsValid)
        {
            var userId = User.FindFirst("userId")?.Value;

        
            try
            {
                await _messagesService.SendMessage(model, userId);
                return Ok();
            }
            catch (UserException)
            {
                return StatusCode(500, new { error = "Пользователя несуществует" });
            }
            
        }
         return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpGet("Messages")]
    public async Task<IActionResult> Messages()
    {
       var userId = User.FindFirst("userId")?.Value;

        var messages = await _messagesService.GetMessages(userId);  
        return Ok(messages);
    }
    
}