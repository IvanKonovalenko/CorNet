using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class Jwt : IJwt
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>(){
            new Claim("userId", user.Email),
        };
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(new TimeSpan(2,0,0)),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B8KslJq3mDtH9FT9S6Ls6xT7PDErwyqmkKKmXzjOYmY=")),SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}