using BL.Exceptions;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserControl _userControl;
        public UserController(IUserControl userControl)
        {
            _userControl = userControl;
        }
        [HttpGet("ShowProfile")]
        public async Task<IActionResult> ShowProfile(string email)
        {
            if (!ModelState.IsValid) return StatusCode(500, new { error = "Данные невалидны" });
            try
            {
                return Ok(await _userControl.ShowProfile(email));
            }
            catch (UserNotExistException)
            {
                return StatusCode(500, new { error = "Пользователь с таким email не зарегистрирован" });
            }
        }
        [HttpGet("ShowOrganizations")]
        public async Task<IActionResult> ShowOrganizations(string email)
        {
            if (!ModelState.IsValid) return StatusCode(500, new { error = "Данные невалидны" });
            try
            {
                return Ok(await _userControl.ShowOrganizations(email));
            }
            catch (UserNotExistException)
            {
                return StatusCode(500, new { error = "Пользователь с таким email не зарегистрирован" });
            }
        }
    }
}
