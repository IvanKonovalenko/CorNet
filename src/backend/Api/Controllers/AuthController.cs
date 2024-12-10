using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly IAuth _auth;
    private readonly IJwt _jwt;

    public AuthController(IAuth auth, IJwt jwt)
    {
        _auth=auth;
        _jwt=jwt;
    }
    [HttpPost("Register")]
    public async Task<IActionResult> Register(AuthModel model)
    {
        if(ModelState.IsValid)
        {
            try
            {
                await _auth.Register(model);
                return Ok();
            }
            catch (DuplicateEmailException)
            {
                return StatusCode(500, new { error = "Email уже зарегестрирован" });
            }
            catch(OrganizationException)
            {
                return StatusCode(500, new { error = "Неверный код организации" });
            }
        }
        return StatusCode(500, new { error = "Данные невалидны" });

    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user=await _auth.Authenticate(model.Email!,model.Password!);
                
                return Ok(_jwt.GenerateToken(user));
            }
            catch(AuthorizationExeception) 
            {
                return StatusCode(500, new { error = "Неверный  Email или Пароль" });
            }        
        }
        return StatusCode(500, new { error = "Данные невалидны" });
    }
}