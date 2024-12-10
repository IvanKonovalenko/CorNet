
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService=organizationService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateOrganization(OrganizationModel model)
    {
       if(ModelState.IsValid)
       {
        try
        {
            await _organizationService.CreateOrganization(model);
            return Ok();
        }
        catch (OrganizationException)
        {
            return StatusCode(500, new { error = "Организация уже существует" });
        }
            
       }
       return StatusCode(500, new { error = "Данные невалидны" });
    }
    
    [HttpGet("Users")]
    [Authorize]
    public async Task<IActionResult> ShowOrganizationUsers()
    {
        var userId = User.FindFirst("userId")?.Value;

        var users = await _organizationService.GetUsers(userId);

        return Ok(users);
    }
}