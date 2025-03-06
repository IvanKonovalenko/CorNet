using BL.Exceptions;
using BL.Interfaces;
using BL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationConroller: ControllerBase
{
    private readonly IOrganizationControl _organizationControl;
    public OrganizationConroller(IOrganizationControl organizationControl)
    {
        _organizationControl = organizationControl;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(OrganizationModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.CreateOrganization(model, email);
                return Ok();
            }
            catch (DuplicateOrganizationException)
            {
                return StatusCode(500, new { error = "Организация c таким кодом уже зарегистрирована" });
            }
            catch (AuthorizationExeception)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpPost("SendRequest")]
    public async Task<IActionResult> SendRequestOrganization(OrganizationRequestModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.SendRequestOrganization(model, email);
                return Ok();
            }
            catch (AuthorizationExeception)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExsistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (RequestExsistsException)
            {
                return StatusCode(500, new { error = "Запрос в эту организацию уже отправлен" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
}