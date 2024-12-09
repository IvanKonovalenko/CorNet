using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwt
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>(){
            new Claim("userId",user.UserId.ToString()),
            new Claim("email", user.Email),
        };
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(new TimeSpan(2)),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wdsfgerwbvervre")),SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}