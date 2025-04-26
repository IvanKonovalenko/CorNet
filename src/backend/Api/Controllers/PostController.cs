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
                    await _postControl.CreatePost(model,code, email!);
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
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string code, int postId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("email")?.Value;
                    await _postControl.DeletePost(code, email!, postId);
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
                    return StatusCode(500, new { error = "Удалить пост может только владелец или администратор" });
                }
                catch (PostNotExistException)
                {
                    return StatusCode(500, new { error = "Пост несуществует" });
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
        [HttpGet("ShowComments")]
        public async Task<IActionResult> ShowComments(string code, int postId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("email")?.Value;
                    return Ok(await _postControl.ShowComments(code, email!, postId));
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
                catch (PostNotExistException)
                {
                    return StatusCode(500, new { error = "Пост несуществует" });
                }
            }
            return StatusCode(500, new { error = "Данные невалидны" });
        }
        [HttpPost("CreateComment")]
        public async Task<IActionResult> CreateComment(string code, int postId, string text)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("email")?.Value;
                    await _postControl.CreateComment(code, email!, postId, text);
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
                catch (UserNotExistInOrganizationException)
                {
                    return StatusCode(500, new { error = "Пользователь не состоит в организации" });
                }
                catch (PostNotExistException)
                {
                    return StatusCode(500, new { error = "Пост несуществует" });
                }
            }
            return StatusCode(500, new { error = "Данные невалидны" });
        }
    }
}
