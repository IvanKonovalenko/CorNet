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
                return Ok();
            }
            catch(OrganizationException)
            {
                
            }
        }
        return Ok();

    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login(AuthModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user=await _auth.Authenticate(model.Email!, model.Password!);
                
                return Ok(_jwt.GenerateToken(user));
            }
            catch(AuthorizationExeception) 
            {
               ModelState.AddModelError("Email","Email или Пароль неверные");
            }        
        }
        return Ok();
    }
}