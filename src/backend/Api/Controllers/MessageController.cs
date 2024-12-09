using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
class MessageController : ControllerBase
{
    public MessageController()
    {
        
    }
    [HttpPost("Login")]
    
}