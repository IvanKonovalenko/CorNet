using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
class MessageController : ControllerBase
{
    private readonly IMessagesService _messagesService;
    public MessageController(IMessagesService messagesService)
    {
        _messagesService= messagesService;
    }
    [HttpPost("SendMessage")]
    public async Task<IActionResult> SendMessage(MessageModel model)
    {
        var userId = User.FindFirst("userId")?.Value;
        var email = User.FindFirst("email")?.Value;
        
        try
        {
            await _messagesService.SendMessage(model, email);
            return Ok();
        }
        catch (UserException)
        {
            return StatusCode(500, new { error = "Пользователя несуществует" });
        }

    }
    [HttpGet("Messages")]
    public async Task<IActionResult> Messages()
    {
        var userId = User.FindFirst("userId")?.Value;
        var email = User.FindFirst("email")?.Value;


        return Ok();
    }
    
}