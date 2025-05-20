using BL.Exceptions;
using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create(string text, string code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("idemail")?.Value;
                    await _postControl.CreatePost(text, code, email!);
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
                    var email = User.FindFirst("idemail")?.Value;
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
        [HttpPost("Like")]
        public async Task<IActionResult> Like(string code, int postId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("idemail")?.Value;
                    await _postControl.Like(code, email!, postId);
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
        [HttpPost("DisLike")]
        public async Task<IActionResult> DisLike(string code, int postId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("idemail")?.Value;
                    await _postControl.DisLike(code, email!, postId);
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
        [HttpGet("ShowPosts")]
        public async Task<IActionResult> ShowPosts(string code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("idemail")?.Value;
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
                    var email = User.FindFirst("idemail")?.Value;
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
                    var email = User.FindFirst("idemail")?.Value;
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
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(string code, int postId, int CommentId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst("idemail")?.Value;
                    await _postControl.DeleteComment(code, email!, postId, CommentId);
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
                catch (PostNotExistException)
                {
                    return StatusCode(500, new { error = "Пост несуществует" });
                }
                catch (CommentNotExistException)
                {
                    return StatusCode(500, new { error = "Комментарий несуществует" });
                }
                catch (RoleAccessException)
                {
                    return StatusCode(500, new { error = "Удалить комментарий может только владелец или администратор" });
                }
            }
            return StatusCode(500, new { error = "Данные невалидны" });
        }
    }
}
