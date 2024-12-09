using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
class OrganizationController : ControllerBase
{
    public OrganizationController()
    {
        
    }
}