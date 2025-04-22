using BL.Exceptions;
using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BL.Models.Post;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostControl _postControl;
        public PostController(IPostControl postControl)
        {
            this._postControl = postControl;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePostModel model, string code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("email")?.Value;
                    await _postControl.Create(model,code, email!);
                    return Ok();
                }
                catch (OrganizationNotExistsException)
                {
                    return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
                }
                catch (AuthorizationException)
                {
                    return StatusCode(500, new { error = "Ошибка авторизации" });
                }
                catch (RoleAccessException)
                {
                    return StatusCode(500, new { error = "Создать пост может только владелец или администратор" });
                }
            }
            return StatusCode(500, new { error = "Данные невалидны" });
        }
        [HttpGet("ShowPosts")]
        public async Task<IActionResult> ShowPosts(string code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("email")?.Value;
                    return Ok(await _postControl.ShowPosts(code, email!));
                }
                catch (OrganizationNotExistsException)
                {
                    return StatusCode(500, new { error = "Организации с таким кодом не сществует" });
                }
                catch (AuthorizationException)
                {
                    return StatusCode(500, new { error = "Ошибка авторизации" });
                }
                catch (UserNotExistInOrganizationException)
                {
                    return StatusCode(500, new { error = "Пользователь не состоит в организации" });
                }
            }
            return StatusCode(500, new { error = "Данные невалидны" });
        }
    }
}
