using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Auth;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuth _auth;
    private readonly IJwt _jwt;

    public AuthController(IAuth auth, IJwt jwt)
    {
        _auth = auth;
        _jwt = jwt;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid) return StatusCode(500, new { error = "Данные невалидны" });
        try
        {
            await _auth.Register(model);
            return Ok();
        }
        catch (DuplicateEmailException)
        {
           return StatusCode(500, new { error = "Email уже зарегестрирован" });
        }
        

    }

    [HttpPost("Login")]
    public async Task<IActionResult> Authenticate(AuthModel model)
    {
        if (!ModelState.IsValid) return StatusCode(500, new { error = "Данные невалидны" });
        try
        {
            var user = await _auth.Authenticate(model);

            return Ok(_jwt.GenerateToken(user));
        }
        catch (AuthorizationException)
        {
            return StatusCode(500, new { error = "Неверный  Email или Пароль" });
        }
    }
}