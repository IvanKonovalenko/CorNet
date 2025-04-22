using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationController: ControllerBase
{
    private readonly IOrganizationControl _organizationControl;
    public OrganizationController(IOrganizationControl organizationControl)
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
                await _organizationControl.Create(model, email!);
                return Ok();
            }
            catch (DuplicateOrganizationException)
            {
                return StatusCode(500, new { error = "Организация c таким кодом уже зарегистрирована" });
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpPost("SendRequest")]
    public async Task<IActionResult> SendRequest(string code)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.SendRequest(code, email!);
                return Ok();
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (RequestExistsException)
            {
                return StatusCode(500, new { error = "Запрос в эту организацию уже отправлен" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpGet("ShowRequests")]
    public async Task<IActionResult> ShowRequests(string code)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                return Ok(await _organizationControl.ShowRequests(code, email!));
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (RoleAccessException)
            {
                return StatusCode(500, new { error = "Просмотреть запросы может только владелец или администратор" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpPost("AcceptRequest")]
    public async Task<IActionResult> AcceptRequest(int OrganizationRequestId, string code)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.AcceptRequest(OrganizationRequestId, code, email!);
                return Ok();
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (OrganizationRequestNotExistsException)
            {
                return StatusCode(500, new { error = "Запроса не существует" });
            }
            catch (RoleAccessException)
            {
                return StatusCode(500, new { error = "Принять запрос может только владелец или администратор" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpPost("RefuseRequest")]
    public async Task<IActionResult> RefuseRequest(int OrganizationRequestId, string code)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.RefuseRequest(OrganizationRequestId, code, email!);
                return Ok();
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (OrganizationRequestNotExistsException)
            {
                return StatusCode(500, new { error = "Запроса не существует" });
            }
            catch (RoleAccessException)
            {
                return StatusCode(500, new { error = "Отклонить запрос может только владелец или администратор" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpGet("ShowUsers")]
    public async Task<IActionResult> ShowUsers(string code)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                return Ok(await _organizationControl.ShowUsers(code, email!));
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (UserNotExistInOrganizationException)
            {
                return StatusCode(500, new { error = "Пользователь не состоит в организации" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpGet("DeleteUser")]
    public async Task<IActionResult> DeleteUser(string code, string emailDeleteUser)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.DeleteUser(code, email!, emailDeleteUser!);
                return Ok();
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (UserNotExistInOrganizationException)
            {
                return StatusCode(500, new { error = "Пользователь не состоит в организации" });
            }
            catch (RoleAccessException)
            {
                return StatusCode(500, new { error = "Удалить пользователя может только владелец или администратор" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
    [HttpGet("ChangeRoleUser")]
    public async Task<IActionResult> ChangeRoleUser(string code, string emailRoleUser, Role role)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var email = User.FindFirst("email")?.Value;
                await _organizationControl.ChangeRoleUser(code, email!, emailRoleUser!, role);
                return Ok();
            }
            catch (AuthorizationException)
            {
                return StatusCode(500, new { error = "Ошибка авторизации" });
            }
            catch (OrganizationNotExistsException)
            {
                return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
            }
            catch (UserNotExistInOrganizationException)
            {
                return StatusCode(500, new { error = "Пользователь не состоит в организации" });
            }
            catch (RoleAccessException)
            {
                return StatusCode(500, new { error = "Изменить роль пользователя может только владелец" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }

}