using BL.Exceptions;
using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageControl _messageControl;

    public MessageController(IMessageControl messageControl)
    {
        _messageControl = messageControl;
    }

    [HttpGet("GetMessages")]
    public async Task<IActionResult> GetMessages(string receiver)
    {
        if (!ModelState.IsValid) return StatusCode(500, new { error = "Данные невалидны" });
        try
        {
            var email = User.FindFirst("idemail")?.Value;
            return Ok(await _messageControl.GetMessages(email!, receiver));
        }
        catch (AuthorizationException)
        {
            return StatusCode(500, new { error = "Ошибка авторизации" });
        }
        catch (ReceiverException)
        {
            return StatusCode(500, new { error = "Получателя не сущесетвует" });
        }
    }
}
